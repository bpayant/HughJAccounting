namespace HughJAccounting.Domain.Audit;

public sealed class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public Guid? UserId { get; set; }

    public AuditEventType EventType { get; set; }

    public required string EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public string? Summary { get; set; }

    public string? MetadataJson { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
}