#region Using Statements

using HughJAccounting.Infrastructure;
using HughJAccounting.Infrastructure.Identity;
using HughJAccounting.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

#endregion

// -----------------------------------------------------------------------------
// Build the application host.
// This reads configuration files, environment variables, command-line args,
// and prepares the service container.
// -----------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Register HughJAccounting infrastructure services.
// This is where our PostgreSQL DbContext is registered through AddInfrastructure.
// -----------------------------------------------------------------------------
builder.Services.AddInfrastructure(builder.Configuration);

// -----------------------------------------------------------------------------
// Register ASP.NET Core Identity API endpoints.
// This adds user registration/login/token infrastructure using ApplicationUser.
// It also connects Identity to our EF Core DbContext through AddEntityFrameworkStores.
// -----------------------------------------------------------------------------
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<HughJAccountingDbContext>();

// -----------------------------------------------------------------------------
// Register authorization services.
// This supports [Authorize] attributes and later policy-based permissions.
// -----------------------------------------------------------------------------
builder.Services.AddAuthorization();

// -----------------------------------------------------------------------------
// Register MVC/API controllers.
// This is what enables controller classes like OrganizationsController.
// -----------------------------------------------------------------------------
builder.Services.AddControllers();

// -----------------------------------------------------------------------------
// Register OpenAPI support.
// This creates the API description document in development.
// -----------------------------------------------------------------------------
builder.Services.AddOpenApi();

// -----------------------------------------------------------------------------
// Build the configured application.
// After this point, we configure the HTTP request pipeline.
// -----------------------------------------------------------------------------
var app = builder.Build();

// -----------------------------------------------------------------------------
// Development-only API documentation endpoint.
// This should not be exposed the same way in production.
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// -----------------------------------------------------------------------------
// Redirect HTTP requests to HTTPS when applicable.
// This should happen before authentication/authorization processing.
// -----------------------------------------------------------------------------
app.UseHttpsRedirection();

// -----------------------------------------------------------------------------
// Authentication determines who the user is.
// Authorization determines what that authenticated user is allowed to do.
// Authentication must come before Authorization.
// -----------------------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// -----------------------------------------------------------------------------
// Map ASP.NET Core Identity endpoints.
// This creates endpoints such as:
//   POST /register
//   POST /login
// These are used to create users and obtain access tokens.
// -----------------------------------------------------------------------------
app.MapIdentityApi<ApplicationUser>();

// -----------------------------------------------------------------------------
// Map our application API controllers.
// This includes endpoints like:
//   GET /api/organizations
//   POST /api/organizations
// -----------------------------------------------------------------------------
app.MapControllers();

// -----------------------------------------------------------------------------
// Start the web application.
// -----------------------------------------------------------------------------
app.Run();
