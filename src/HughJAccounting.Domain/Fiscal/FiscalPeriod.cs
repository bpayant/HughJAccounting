namespace HughJAccounting.Domain.Fiscal;

public sealed class FiscalPeriod
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid AccountingEntityId { get; set; }

    public required string Name { get; set; }

    public int FiscalYear { get; set; }

    public int PeriodNumber { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsClosed { get; set; }

    public DateTimeOffset? ClosedAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}