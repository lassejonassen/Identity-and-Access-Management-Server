using Common.Tokens;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Abstractions.Services;

public interface ITokenRevocationService
{
    Task<TokenRecovationResponse> RevokeTokenAsync(HttpContext httpContext, string clientId);
}
