using Application.Contracts.Responses;
using Application.Contracts.Tokens;

namespace Infrastructure.Services.Interfaces;

public interface ITokenIntrospectionService
{
    Task<TokenIntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
}
