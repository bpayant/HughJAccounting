using HughJAccounting.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HughJAccounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HughJAccounting");

        services.AddDbContext<HughJAccountingDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}