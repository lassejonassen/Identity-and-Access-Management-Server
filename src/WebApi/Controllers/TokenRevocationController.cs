using Infrastructure.Services.Interfaces;
using Infrastructure.Validations.TokenRevocation;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/token-revocation")]
[ApiController]
public class TokenRevocationController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenRevocationValidation _tokenRevocationValidation;
    private readonly ITokenRevocationService _tokenRevocationService;

    public TokenRevocationController(
        IHttpContextAccessor httpContextAccessor,
        ITokenRevocationValidation tokenRevocationValidation,
        ITokenRevocationService tokenRevocationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenRevocationValidation = tokenRevocationValidation;
        _tokenRevocationService = tokenRevocationService;
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        var clientValidationResult = await _tokenRevocationValidation.ValidateAsync(_httpContextAccessor.HttpContext!);

        if (!clientValidationResult.Succeeded)
        {
            return Unauthorized(clientValidationResult.Error);
        }

        var tokenRevokeResult = await _tokenRevocationService.RevokeTokenAsync(_httpContextAccessor.HttpContext!, clientValidationResult.Client.ClientId.ToString());

        if (!tokenRevokeResult.Succeeded)
        {
            return NotFound(tokenRevokeResult.Error);
        }

        return Ok("token_revoked");
    }
}
