using HughJAccounting.Domain.Accounting;
using HughJAccounting.Domain.Audit;
using HughJAccounting.Domain.Entities;
using HughJAccounting.Domain.Fiscal;
using HughJAccounting.Domain.Tenancy;

using Microsoft.EntityFrameworkCore;

namespace HughJAccounting.Infrastructure.Persistence;

public sealed class HughJAccountingDbContext : DbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTenancy(modelBuilder);
        ConfigureEntities(modelBuilder);
        ConfigureAccounting(modelBuilder);
        ConfigureFiscal(modelBuilder);
        ConfigureAudit(modelBuilder);
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