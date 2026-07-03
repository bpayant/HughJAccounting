namespace HughJAccounting.Domain.Accounting;

public sealed class JournalEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid AccountingEntityId { get; set; }

    public Guid? FiscalPeriodId { get; set; }

    public DateOnly EntryDate { get; set; }

    public string? ReferenceNumber { get; set; }

    public required string Memo { get; set; }

    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;

    public JournalEntrySource Source { get; set; } = JournalEntrySource.Manual;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? PostedAtUtc { get; set; }

    public List<JournalLine> Lines { get; set; } = [];
}