using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Modules.Clients;
using Domain.Modules.OAuth;
using Infrastructure.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;
public class CodeStoreService : ICodeStoreService
{
    private readonly ConcurrentDictionary<string, AuthorizationCode> _codeIssued = new ConcurrentDictionary<string, AuthorizationCode>();
    private readonly ClientStore _clientStore = new ClientStore();

    public string? GenerateAuthorizationCode(AuthorizationCode authorizationCode)
    {
        var client = _clientStore.Clients.Where(x => x.ClientId.ToString() == authorizationCode.ClientId).SingleOrDefault();

        if (client != null)
        {
            var rand = RandomNumberGenerator.Create();
            byte[] bytes = new byte[32];
            rand.GetBytes(bytes);
            var code = Base64UrlEncoder.Encode(bytes);

            _codeIssued[code] = authorizationCode;

            return code;
        }
        return null;
    }

    public AuthorizationCode? GetClientDataByCode(string key)
    {
        AuthorizationCode authorizationCode;
        if (_codeIssued.TryGetValue(key, out authorizationCode))
        {
            return authorizationCode;
        }
        return null;
    }

    public AuthorizationCode? RemoveClientDataByCode(string key)
    {

        var isRemoved = _codeIssued.TryRemove(key, out AuthorizationCode? authorizationCode);

        if (isRemoved)
        {
            return authorizationCode;
        }

        return null;
    }

    public AuthorizationCode? UpdatedClientDataByCode(string key, ClaimsPrincipal claimsPrincipal, IList<string> requestdScopes)
    {
        var oldValue = GetClientDataByCode(key);

        if (oldValue != null)
        {
            var client = _clientStore.Clients.FirstOrDefault(x => x.ClientId.ToString() == oldValue.ClientId);

            if (client != null)
            {
                var clientScope = (from m in client.AllowedScopes
                                   where requestdScopes.Contains(m)
                                   select m).ToList();

                if (!clientScope.Any())
                {
                    return null;
                }

                AuthorizationCode newValue = new AuthorizationCode
                {
                    ClientId = oldValue.ClientId,
                    CreationTime = oldValue.CreationTime,
                    IsOpenId = requestdScopes.Contains("openId") || requestdScopes.Contains("profile"),
                    RedirectUri = oldValue.RedirectUri,
                    RequestedScopes = requestdScopes,
                    Nonce = oldValue.Nonce,
                    CodeChallenge = oldValue.CodeChallenge,
                    CodeChallengeMethod = oldValue.CodeChallengeMethod,
                    Subject = claimsPrincipal,
                };

                var result = _codeIssued.TryUpdate(key, newValue, oldValue);

                if (result)
                {
                    return newValue;
                }

                return null;
            }
        }

        return null;
    }
}
