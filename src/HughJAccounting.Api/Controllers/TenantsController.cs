using HughJAccounting.Domain.Audit;
using HughJAccounting.Domain.Tenancy;
using HughJAccounting.Infrastructure.Persistence;
using HughJAccounting.Api.Auth;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace HughJAccounting.Api.Controllers;

[Authorize(AuthenticationSchemes = AuthenticationSchemes.IdentityBearer)]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly HughJAccountingDbContext _dbContext;

    public TenantsController(HughJAccountingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> GetTenants(
        CancellationToken cancellationToken)
    {
        var tenants = await _dbContext.Tenants
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TenantResponse(
                x.Id,
                x.Name,
                x.Slug,
                x.IsActive,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantResponse>> GetTenant(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TenantResponse(
                x.Id,
                x.Name,
                x.Slug,
                x.IsActive,
                x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

        if (tenant is null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<TenantResponse>> CreateTenant(
        CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Tenant name is required.");
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? CreateSlug(request.Name)
            : CreateSlug(request.Slug);

        var slugAlreadyExists = await _dbContext.Tenants
            .AnyAsync(x => x.Slug == slug, cancellationToken);

        if (slugAlreadyExists)
        {
            return Conflict($"A tenant with slug '{slug}' already exists.");
        }

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Slug = slug
        };

        _dbContext.Tenants.Add(tenant);

        _dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = tenant.Id,
            EventType = AuditEventType.Created,
            EntityType = AuditedEntityTypes.Tenant,
            EntityId = tenant.Id,
            Summary = $"Created tenant '{tenant.Name}'."
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new TenantResponse(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.IsActive,
            tenant.CreatedAtUtc);

        return CreatedAtAction(
            nameof(GetTenant),
            new { id = tenant.Id },
            response);
    }

    private static string CreateSlug(string value)
    {
        var trimmed = value.Trim().ToLowerInvariant();

        var characters = trimmed
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray();

        var slug = new string(characters);

        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return slug.Trim('-');
    }
}

public sealed record CreateTenantRequest(
    string Name,
    string? Slug);

public sealed record TenantResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);