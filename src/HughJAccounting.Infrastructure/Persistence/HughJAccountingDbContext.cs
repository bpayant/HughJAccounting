using HughJAccounting.Domain.Accounting;
using HughJAccounting.Domain.Audit;
using HughJAccounting.Domain.AccountingEntities;
using HughJAccounting.Domain.Fiscal;
using HughJAccounting.Domain.Organizations;
using HughJAccounting.Domain.Security;
using HughJAccounting.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HughJAccounting.Infrastructure.Persistence;

public sealed class HughJAccountingDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public HughJAccountingDbContext(DbContextOptions<HughJAccountingDbContext> options)
        : base(options)
    {
    }

    // Organization DbSets
    public DbSet<Organization> Organizations => Set<Organization>();
    // Accounting Entity DbSets
    public DbSet<AccountingEntity> AccountingEntities => Set<AccountingEntity>();
    // Accounting DbSets
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    // Fiscal DbSets
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    // Audit DbSets
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    // Security DbSets
    public DbSet<AccessGrant> AccessGrants => Set<AccessGrant>();
    public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();
    public DbSet<OrganizationMembershipRole> OrganizationMembershipRoles => Set<OrganizationMembershipRole>();
    public DbSet<OrganizationRole> OrganizationRoles => Set<OrganizationRole>();
    public DbSet<OrganizationRolePermission> OrganizationRolePermissions => Set<OrganizationRolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentity(modelBuilder);
        ConfigureOrganizations(modelBuilder);
        ConfigureAccountingEntities(modelBuilder);
        ConfigureAccounting(modelBuilder);
        ConfigureFiscal(modelBuilder);
        ConfigureAudit(modelBuilder);
        ConfigureSecurity(modelBuilder);
    }

    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("identity_users");

            entity.Property(x => x.DisplayName)
                .HasMaxLength(200);
        });

        modelBuilder.Entity<IdentityRole<Guid>>()
            .ToTable("identity_roles");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("identity_user_roles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("identity_user_claims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("identity_user_logins");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("identity_role_claims");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("identity_user_tokens");
    }

    private static void ConfigureSecurity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessGrant>(entity =>
        {
            entity.ToTable("access_grants");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PermissionKey)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.ResourceType)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Reason)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.OrganizationId,
                x.UserId,
                x.PermissionKey,
                x.ResourceType,
                x.ResourceId
            });

            entity.HasIndex(x => new
            {
                x.OrganizationId,
                x.AccountingEntityId,
                x.ResourceType,
                x.ResourceId
            });
        });
        modelBuilder.Entity<OrganizationMembership>(entity =>
        {
            entity.ToTable("organization_memberships");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.OrganizationId, x.UserId })
                .IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrganizationRole>(entity =>
        {
            entity.ToTable("organization_roles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DisplayName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => new { x.OrganizationId, x.Name })
                .IsUnique();
        });

        modelBuilder.Entity<OrganizationRolePermission>(entity =>
        {
            entity.ToTable("organization_permissions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PermissionKey)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(x => new { x.OrganizationId, x.OrganizationRoleId, x.PermissionKey })
                .IsUnique();

            entity.HasOne<OrganizationRole>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrganizationMembershipRole>(entity =>
        {
            entity.ToTable("organization_membership_roles");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.OrganizationMembershipId, x.OrganizationRoleId })
                .IsUnique();

            entity.HasOne<OrganizationMembership>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationMembershipId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<OrganizationRole>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureOrganizations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organizations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Slug)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Slug)
                .IsUnique();
        });
    }

    private static void ConfigureAccountingEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountingEntity>(entity =>
        {
            entity.ToTable("accounting_entities");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LegalName)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.DisplayName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.TaxIdLastFour)
                .HasMaxLength(4);

            entity.HasIndex(x => new { x.OrganizationId, x.DisplayName })
                .IsUnique();
        });
    }

    private static void ConfigureAccounting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.AccountNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ReportGroup)
                .HasMaxLength(150);

            entity.HasIndex(x => new { x.OrganizationId, x.AccountNumber })
                .IsUnique();
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.ToTable("journal_entries");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReferenceNumber)
                .HasMaxLength(100);

            entity.Property(x => x.Memo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JournalLine>(entity =>
        {
            entity.ToTable("journal_lines");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Debit)
                .HasPrecision(18, 2);

            entity.Property(x => x.Credit)
                .HasPrecision(18, 2);

            entity.HasIndex(x => new { x.OrganizationId, x.JournalEntryId, x.LineNumber })
                .IsUnique();
        });
    }

    private static void ConfigureFiscal(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FiscalPeriod>(entity =>
        {
            entity.ToTable("fiscal_periods");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => new { x.OrganizationId, x.AccountingEntityId, x.FiscalYear, x.PeriodNumber })
                .IsUnique();
        });
    }

    private static void ConfigureAudit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("audit_events");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.EntityType)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Summary)
                .HasMaxLength(1000);

            entity.Property(x => x.MetadataJson)
                .HasColumnType("jsonb");
        });
    }
}