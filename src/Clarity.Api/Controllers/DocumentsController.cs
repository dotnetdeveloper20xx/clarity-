using Clarity.Application.Documents.Commands.UploadDocument;
using Clarity.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorage;

    public DocumentsController(IMediator mediator, IFileStorageService fileStorage)
    {
        _mediator = mediator;
        _fileStorage = fileStorage;
    }

    [HttpPost]
    [RequestSizeLimit(50_000_000)] // 50MB limit
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
        return NotFound("Download endpoint will be fully implemented with document query handler.");
    }
}

public class DocumentUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public Guid? ClientId { get; set; }
    public Guid? MatterId { get; set; }
    public string? Category { get; set; }
}
