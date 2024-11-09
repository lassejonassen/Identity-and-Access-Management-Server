using Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/discovery")]
[ApiController]
public class DiscoveryEndpointController(IConfiguration configuration) : ControllerBase
{
    [HttpGet(".well-known/openid-configuration")]
    public IActionResult GetConfiguration()
    {
        var response = configuration.GetValue<OpenIdConfiguration>("AppSettings:OpenIdConfiguration");
        return Ok(response);
    }

    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var response = configuration.GetValue<Jwk>("AppSettings:Jwks");
        return Ok(response);
    }
}
