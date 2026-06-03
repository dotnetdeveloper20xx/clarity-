using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Document : AuditableEntity
{
    public Guid? ClientId { get; set; }
    public Guid? MatterId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public Guid? ParentDocumentId { get; set; }
    public string? Category { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Active;
    public Guid UploadedById { get; set; }

    // Navigation
    public Client? Client { get; set; }
    public Matter? Matter { get; set; }
    public ApplicationUser UploadedBy { get; set; } = null!;
}
