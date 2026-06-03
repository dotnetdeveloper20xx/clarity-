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
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid? clientId, [FromForm] Guid? matterId, [FromForm] string? category)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadDocumentCommand
        {
            ClientId = clientId,
            MatterId = matterId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            FileStream = stream,
            Category = category
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        // Simplified — in production, would use a query handler
        return NotFound("Download endpoint will be fully implemented with document query handler.");
    }
}
