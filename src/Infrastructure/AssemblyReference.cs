using Application.Abstractions.Services;
using Domain.Modules.Users;
using Infrastructure.Abstractions.Services;
using Infrastructure.Abstractions.Validations;
using Infrastructure.Ldap;
using Infrastructure.Options;
using Infrastructure.Services;
using Infrastructure.Validations.BearerTokenUsageType;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.DbContexts;

namespace Infrastructure;

public static class AssemblyReference
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddServices();
        services.AddValidations();
        services.AddAspNetCoreIdentity();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<LdapOptionsSetup>();
        services.ConfigureOptions<OAuthServerOptionsSetup>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserManagerService, UserManagerService>();

        return services;
    }

    private static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddScoped<IBearerTokenUsageTypeValidation, BearerTokenUsageTypeValidation>();

        return services;
    }

    private static IServiceCollection AddAspNetCoreIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(o =>
        {
            o.SignIn.RequireConfirmedEmail = false;
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequiredLength = 8;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.User.RequireUniqueEmail = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
