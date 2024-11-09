using Infrastructure.Abstractions.Services;
using Infrastructure.Abstractions.Validations;
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
        return Ok("");
    }
}
