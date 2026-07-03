namespace HughJAccounting.Domain.Security;

public sealed class OrganizationRolePermission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid OrganizationRoleId { get; set; }

    public required string PermissionKey { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}