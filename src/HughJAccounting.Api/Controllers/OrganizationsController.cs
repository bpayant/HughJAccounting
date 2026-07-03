using HughJAccounting.Domain.Audit;
using HughJAccounting.Domain.Organizations;
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
public sealed class OrganizationsController : ControllerBase
{
    private readonly HughJAccountingDbContext _dbContext;

    public OrganizationsController(HughJAccountingDbContext dbContext) 
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationResponse>>> GetOrganizations(
        CancellationToken cancellationToken)
    {
        var organizations = await _dbContext.Organizations
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new OrganizationResponse(
                x.Id,
                x.Name,
                x.Slug,
                x.IsActive,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(organizations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> GetOrganization(
        Guid id,
        CancellationToken cancellationToken)
    {
        var organization = await _dbContext.Organizations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new OrganizationResponse(
                x.Id,
                x.Name,
                x.Slug,
                x.IsActive,
                x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> CreateOrganization(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Organization name is required.");
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? CreateSlug(request.Name)
            : CreateSlug(request.Slug);

        var slugAlreadyExists = await _dbContext.Organizations
            .AnyAsync(x => x.Slug == slug, cancellationToken);

        if (slugAlreadyExists)
        {
            return Conflict($"A organization with slug '{slug}' already exists.");
        }

        var organization = new Organization
        {
            Name = request.Name.Trim(),
            Slug = slug
        };

        _dbContext.Organizations.Add(organization);

        _dbContext.AuditEvents.Add(new AuditEvent
        {
            OrganizationId = organization.Id,
            EventType = AuditEventType.Created,
            EntityType = AuditedEntityTypes.Organization,
            EntityId = organization.Id,
            Summary = $"Created organization '{organization.Name}'."
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new OrganizationResponse(
            organization.Id,
            organization.Name,
            organization.Slug,
            organization.IsActive,
            organization.CreatedAtUtc);

        return CreatedAtAction(
            nameof(GetOrganization),
            new { id = organization.Id },
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

public sealed record CreateOrganizationRequest(
    string Name,
    string? Slug);

public sealed record OrganizationResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);