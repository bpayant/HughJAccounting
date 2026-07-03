namespace HughJAccounting.Domain.Security;

public sealed class TenantPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TenantId { get; set; }

    public Guid TenantRoleId { get; set; }

    public required string PermissionKey { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}