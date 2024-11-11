using Microsoft.Extensions.Options;

namespace WebApi.Extensions.OpenId;

public class OpenIdConfigurationOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<OpenIdConfigurationOptions>
{
    private const string SectionName = "AppSettings:OpenId:Configuration";

    public void Configure(OpenIdConfigurationOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
