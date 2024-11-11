using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Ldap;

public sealed class LdapService : ILdapService
{
    private readonly LdapOptions _ldapOptions;
    private readonly ILogger _logger;

    public LdapService(IOptionsSnapshot<LdapOptions> ldapOptions, ILogger logger)
    {
        _ldapOptions = ldapOptions.Value;
        _logger = logger;
    }
}
