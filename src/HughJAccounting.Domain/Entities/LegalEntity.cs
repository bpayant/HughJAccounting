namespace HughJAccounting.Domain.Entities;

public sealed class LegalEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TenantId { get; set; }

    public required string LegalName { get; set; }

    public required string DisplayName { get; set; }

    public string? TaxIdLastFour { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}