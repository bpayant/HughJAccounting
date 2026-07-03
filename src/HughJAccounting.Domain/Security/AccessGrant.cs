namespace HughJAccounting.Domain.Security;

public sealed class AccessGrant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid? AccountingEntityId { get; set; }

    public Guid? UserId { get; set; }

    public required string PermissionKey { get; set; }

    public required string ResourceType { get; set; }

    public Guid? ResourceId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ExpiresAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public Guid? RevokedByUserId { get; set; }

    public string? Reason { get; set; }
}