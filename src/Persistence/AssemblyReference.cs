using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.DbContexts;
using Persistence.Options;
using Microsoft.EntityFrameworkCore;
using Domain.Modules.Users;
using Persistence.Repositories;

namespace Persistence;

public static class AssemblyReference
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseOptionsSetup>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, dbContextOptionsBuilder) => {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            dbContextOptionsBuilder.UseSqlServer(databaseOptions.ConnectionString, action => {
                action.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
                action.CommandTimeout(databaseOptions.CommandTimeout);
            });

            dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
            dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
