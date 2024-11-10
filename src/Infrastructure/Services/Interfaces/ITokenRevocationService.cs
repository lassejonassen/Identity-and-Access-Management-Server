using Common.Tokens;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Interfaces;

public interface ITokenRevocationService
{
    Task<TokenRecovationResponse> RevokeTokenAsync(HttpContext httpContext, string clientId);
}
