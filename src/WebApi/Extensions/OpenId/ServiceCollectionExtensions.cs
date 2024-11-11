namespace WebApi.Extensions.OpenId;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOpenIdOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<OpenIdConfigurationOptionsSetup>();
        return services;
    }
}
