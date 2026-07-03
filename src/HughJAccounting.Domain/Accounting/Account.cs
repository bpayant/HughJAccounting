namespace HughJAccounting.Domain.Accounting;

public sealed class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid? ParentAccountId { get; set; }

    public required string AccountNumber { get; set; }

    public required string Name { get; set; }

    public AccountType Type { get; set; }

    public NormalBalance NormalBalance { get; set; }

    public string? ReportGroup { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPostingAccount { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}