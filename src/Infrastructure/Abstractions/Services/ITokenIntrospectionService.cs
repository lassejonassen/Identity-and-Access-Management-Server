using Common.Tokens;

namespace Infrastructure.Abstractions.Services;

public interface ITokenIntrospectionService
{
    Task<TokenIntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
}
