using Common.Tokens;

namespace Infrastructure.Services.Interfaces;

public interface ITokenIntrospectionService
{
    Task<TokenIntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
}
