using Infrastructure.Validations.TokenRevocation;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Abstractions.Validations;

public interface ITokenRevocationValidation
{
    Task<TokenRevocationValidationResponse> ValidateAsync(HttpContext context);
}