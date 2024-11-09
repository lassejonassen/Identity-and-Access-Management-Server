using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Ldap;

public sealed class LdapOptionsSetup : IConfigureOptions<LdapOptions>
{
    private const string SectionName = "AppSettings:Federation:Ldap";
    private readonly IConfiguration _configuration;

    public LdapOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(LdapOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
