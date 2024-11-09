using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Options;

public sealed class OAuthServerOptionsSetup : IConfigureOptions<OAuthServerOptions>
{
    private const string SectionName = "AppSettings:OAuthServer";
    private readonly IConfiguration _configuration;

    public OAuthServerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(OAuthServerOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
