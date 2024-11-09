namespace Abstractions;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
