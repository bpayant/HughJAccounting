namespace HughJAccounting.Domain.AccountingEntities;

public sealed class AccountingEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public required string LegalName { get; set; }

    public required string DisplayName { get; set; }

    public string? TaxIdLastFour { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}