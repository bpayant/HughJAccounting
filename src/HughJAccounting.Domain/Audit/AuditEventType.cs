namespace HughJAccounting.Domain.Audit;

public enum AuditEventType
{
    Created = 1,
    Updated = 2,
    Posted = 3,
    Voided = 4,
    Approved = 5,
    Rejected = 6,
    Deleted = 7,
    Imported = 8,
    Exported = 9,
    Locked = 10,
    Unlocked = 11
}