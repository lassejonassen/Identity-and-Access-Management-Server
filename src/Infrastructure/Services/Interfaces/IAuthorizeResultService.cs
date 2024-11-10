using Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Interfaces;

public interface IAuthorizeResultService
{
    AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest);
    Task<TokenResponse> GenerateToken(TokenRequest tokenRequest);
}