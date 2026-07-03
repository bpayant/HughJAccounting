using HughJAccounting.Domain.Accounting;
using HughJAccounting.Domain.Audit;
using HughJAccounting.Domain.Entities;
using HughJAccounting.Domain.Fiscal;
using HughJAccounting.Domain.Tenancy;
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

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<LegalEntity> LegalEntities => Set<LegalEntity>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<TenantRole> TenantRoles => Set<TenantRole>();
    public DbSet<TenantPermission> TenantPermissions => Set<TenantPermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentity(modelBuilder);
        ConfigureTenancy(modelBuilder);
        ConfigureEntities(modelBuilder);
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
        modelBuilder.Entity<TenantMembership>(entity =>
        {
            entity.ToTable("tenant_memberships");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.TenantId, x.UserId })
                .IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TenantRole>(entity =>
        {
            entity.ToTable("tenant_roles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DisplayName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.Name })
                .IsUnique();
        });

        modelBuilder.Entity<TenantPermission>(entity =>
        {
            entity.ToTable("tenant_permissions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PermissionKey)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(x => new { x.TenantId, x.TenantRoleId, x.PermissionKey })
                .IsUnique();

            entity.HasOne<TenantRole>()
                .WithMany()
                .HasForeignKey(x => x.TenantRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTenancy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");

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

    private static void ConfigureEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LegalEntity>(entity =>
        {
            entity.ToTable("legal_entities");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LegalName)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.DisplayName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.TaxIdLastFour)
                .HasMaxLength(4);

            entity.HasIndex(x => new { x.TenantId, x.DisplayName })
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

            entity.HasIndex(x => new { x.TenantId, x.AccountNumber })
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

            entity.HasIndex(x => new { x.TenantId, x.JournalEntryId, x.LineNumber })
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

            entity.HasIndex(x => new { x.TenantId, x.LegalEntityId, x.FiscalYear, x.PeriodNumber })
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