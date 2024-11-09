using Common.Tokens;
using Infrastructure.Validations.TokenIntrospection;

namespace Infrastructure.Abstractions.Validations;

public interface ITokenIntrospectionValidation
{
    Task<TokenIntrospectionValidationResponse> ValidateAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
}
