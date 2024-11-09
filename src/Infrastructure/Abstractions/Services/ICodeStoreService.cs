using System.Security.Claims;
using Domain.Modules.OAuth;

namespace Infrastructure.Abstractions.Services;

public interface ICodeStoreService
{
    string? GenerateAuthorizationCode(AuthorizationCode authorizationCode);
    AuthorizationCode? GetClientDataByCode(string key);
    AuthorizationCode? UpdatedClientDataByCode(string key, ClaimsPrincipal claimsPrincipal, IList<string> requestdScopes);
    AuthorizationCode? RemoveClientDataByCode(string key);
}
