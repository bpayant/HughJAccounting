namespace HughJAccounting.Domain.Security;

public sealed class OrganizationMembership
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DeactivatedAtUtc { get; set; }
}