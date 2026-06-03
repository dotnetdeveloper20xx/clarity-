namespace Clarity.Domain.Common;

public abstract class AuditableEntity : BaseEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public byte[]? RowVersion { get; set; }
}
