using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.TokenRevocation;

public interface ITokenRevocationValidation
{
    Task<TokenRevocationValidationResponse> ValidateAsync(HttpContext context);
}