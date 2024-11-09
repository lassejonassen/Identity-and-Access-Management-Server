using Application.Abstractions.Services;
using Infrastructure.Abstractions.Validations;
using Infrastructure.Ldap;
using Infrastructure.Options;
using Infrastructure.Services;
using Infrastructure.Validations.BearerTokenUsageType;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class AssemblyReference
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddServices();
        services.AddValidations();

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

        return services;
    }

    private static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddScoped<IBearerTokenUsageTypeValidation, BearerTokenUsageTypeValidation>();

        return services;
    }

    private static IServiceCollection AddAspNetCoreIdentity(this IServiceCollection services)
    {

    }
}
