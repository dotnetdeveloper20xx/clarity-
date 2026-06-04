using Clarity.Application.Documents.Commands.UploadDocument;
using Clarity.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorage;
    private readonly IApplicationDbContext _context;

    public DocumentsController(IMediator mediator, IFileStorageService fileStorage, IApplicationDbContext context)
    {
        _mediator = mediator;
        _fileStorage = fileStorage;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] Guid? matterId, [FromQuery] Guid? clientId)
    {
        var query = _context.Documents.AsNoTracking().Where(d => !d.IsDeleted);

        if (matterId.HasValue) query = query.Where(d => d.MatterId == matterId.Value);
        if (clientId.HasValue) query = query.Where(d => d.ClientId == clientId.Value);

        var docs = await query
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new
            {
                d.Id, d.FileName, d.ContentType, d.FileSizeBytes,
                d.Version, d.Category, d.Status, d.CreatedAt,
                d.MatterId, d.ClientId
            })
            .Take(50)
            .ToListAsync();

        return Ok(docs);
    }

    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] DocumentUploadRequest request)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest("No file provided.");

        using var stream = request.File.OpenReadStream();
        var result = await _mediator.Send(new UploadDocumentCommand
        {
            ClientId = request.ClientId,
            MatterId = request.MatterId,
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileSizeBytes = request.File.Length,
            FileStream = stream,
            Category = request.Category
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var document = await _context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (document is null)
            return NotFound(new { message = "Document not found." });

        try
        {
            var stream = await _fileStorage.DownloadAsync(document.StoragePath);
            return File(stream, document.ContentType, document.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "Document file not found in storage." });
        }
    }
}

public class DocumentUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public Guid? ClientId { get; set; }
    public Guid? MatterId { get; set; }
    public string? Category { get; set; }
}
