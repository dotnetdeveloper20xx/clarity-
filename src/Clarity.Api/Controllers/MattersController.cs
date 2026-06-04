using Clarity.Application.Common.Interfaces;
using Clarity.Application.Matters.Commands.CreateMatter;
using Clarity.Application.Matters.Commands.UpdateMatterStatus;
using Clarity.Application.Matters.Queries.GetMatter;
using Clarity.Application.Matters.Queries.GetMatters;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MattersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public MattersController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMatters(
        [FromQuery] string? searchTerm,
        [FromQuery] MatterStatus? status,
        [FromQuery] Guid? clientId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetMattersQuery
        {
            SearchTerm = searchTerm,
            Status = status,
            ClientId = clientId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMatter(Guid id)
    {
        var result = await _mediator.Send(new GetMatterQuery(id));
        return result.Succeeded ? Ok(result.Data) : NotFound(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatter([FromBody] CreateMatterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetMatter), new { id = result.Data }, result.Data)
            : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateMatterStatusCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        var result = await _mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}/notes")]
    public async Task<IActionResult> GetNotes(Guid id)
    {
        var notes = await _context.MatterNotes
            .Where(n => n.MatterId == id && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new { n.Id, n.Content, n.IsClientVisible, n.CreatedAt })
            .ToListAsync();
        return Ok(notes);
    }

    [HttpGet("{id:guid}/tasks")]
    public async Task<IActionResult> GetTasks(Guid id)
    {
        var tasks = await _context.MatterTasks
            .Where(t => t.MatterId == id && !t.IsDeleted)
            .OrderBy(t => t.Status).ThenBy(t => t.DueDate)
            .Select(t => new { t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CompletedAt })
            .ToListAsync();
        return Ok(tasks);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> CreateNote(Guid id, [FromBody] CreateNoteRequest request)
    {
        var note = new Clarity.Domain.Entities.MatterNote
        {
            MatterId = id,
            Content = request.Content,
            IsClientVisible = request.IsClientVisible,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty // Will be set from current user in production
        };
        _context.MatterNotes.Add(note);
        await _context.SaveChangesAsync();
        return Ok(note.Id);
    }

    [HttpPost("{id:guid}/tasks")]
    public async Task<IActionResult> CreateTask(Guid id, [FromBody] CreateTaskRequest request)
    {
        var task = new Clarity.Domain.Entities.MatterTask
        {
            MatterId = id,
            Title = request.Title,
            Description = request.Description,
            Priority = (Clarity.Domain.Enums.TaskPriority)request.Priority,
            DueDate = DateOnly.Parse(request.DueDate),
            AssigneeId = request.AssigneeId != Guid.Empty ? request.AssigneeId : Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Default to Sarah
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty
        };
        _context.MatterTasks.Add(task);
        await _context.SaveChangesAsync();
        return Ok(task.Id);
    }
}

public record CreateNoteRequest
{
    public string Content { get; init; } = string.Empty;
    public bool IsClientVisible { get; init; }
}

public record CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Priority { get; init; } = 1;
    public string DueDate { get; init; } = string.Empty;
    public Guid AssigneeId { get; init; }
}
