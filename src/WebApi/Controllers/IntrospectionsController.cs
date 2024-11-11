using Application.Contracts.Tokens;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/introspections")]
[ApiController]
public class IntrospectionsController
    (ITokenIntrospectionService tokenIntrospectionService): ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> TokenIntrospect(TokenIntrospectionRequest tokenIntrospectRequest)
    {
        var result = await tokenIntrospectionService.IntrospectTokenAsync(tokenIntrospectRequest);
        return Ok(result);
    }
}
