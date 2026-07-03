namespace HughJAccounting.Domain.Accounting;

public sealed class JournalLine
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid JournalEntryId { get; set; }

    public Guid AccountId { get; set; }

    public string? Description { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public int LineNumber { get; set; }
}