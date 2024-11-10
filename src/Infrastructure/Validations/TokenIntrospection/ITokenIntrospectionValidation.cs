using Common.Tokens;

namespace Infrastructure.Validations.TokenIntrospection;

public interface ITokenIntrospectionValidation
{
    Task<TokenIntrospectionValidationResponse> ValidateAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
}
