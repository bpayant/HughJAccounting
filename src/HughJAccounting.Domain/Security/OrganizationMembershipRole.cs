namespace HughJAccounting.Domain.Security;

public sealed class OrganizationMembershipRole
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationMembershipId { get; set; }

    public Guid OrganizationRoleId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}