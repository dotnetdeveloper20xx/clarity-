using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ExportController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("clients")]
    public async Task<IActionResult> ExportClients()
    {
        var clients = await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new { c.ReferenceNumber, c.Name, c.Email, c.Phone, c.City, c.PostCode, Status = c.Status.ToString(), Type = c.ClientType.ToString() })
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Reference,Name,Email,Phone,City,PostCode,Status,Type");
        foreach (var c in clients)
            csv.AppendLine($"{c.ReferenceNumber},{EscapeCsv(c.Name)},{c.Email},{c.Phone},{c.City},{c.PostCode},{c.Status},{c.Type}");

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "clients-export.csv");
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> ExportInvoices()
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => !i.IsDeleted)
            .Include(i => i.Client)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new { i.InvoiceNumber, ClientName = i.Client.Name, Status = i.Status.ToString(), i.TotalAmount, i.PaidAmount, Outstanding = i.TotalAmount - i.PaidAmount, i.IssueDate, i.DueDate })
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("InvoiceNumber,Client,Status,Total,Paid,Outstanding,IssueDate,DueDate");
        foreach (var i in invoices)
            csv.AppendLine($"{i.InvoiceNumber},{EscapeCsv(i.ClientName)},{i.Status},{i.TotalAmount},{i.PaidAmount},{i.Outstanding},{i.IssueDate},{i.DueDate}");

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "invoices-export.csv");
    }

    [HttpGet("time-entries")]
    public async Task<IActionResult> ExportTimeEntries([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
    {
        var query = _context.TimeEntries.AsNoTracking().Where(t => !t.IsDeleted);
        if (fromDate.HasValue) query = query.Where(t => t.Date >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(t => t.Date <= toDate.Value);

        var entries = await query
            .OrderByDescending(t => t.Date)
            .Select(t => new { t.Date, MatterRef = t.Matter.ReferenceNumber, UserName = t.User.FirstName + " " + t.User.LastName, t.DurationMinutes, t.Description, t.IsBillable, Status = t.Status.ToString(), t.RateAmount })
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Date,Matter,User,Minutes,Description,Billable,Status,Rate");
        foreach (var e in entries)
            csv.AppendLine($"{e.Date},{e.MatterRef},{EscapeCsv(e.UserName)},{e.DurationMinutes},{EscapeCsv(e.Description)},{e.IsBillable},{e.Status},{e.RateAmount}");

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "time-entries-export.csv");
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
