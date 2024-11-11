using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Extensions.OpenId;

namespace WebApi.Controllers;

[Route("api/discovery")]
[ApiController]
public class DiscoveryEndpointController(IOptionsSnapshot<OpenIdConfigurationOptions> options) : ControllerBase
{
    private readonly OpenIdConfigurationOptions _options = options.Value;

    [HttpGet(".well-known/openid-configuration")]
    public IActionResult GetConfiguration()
    {
        return Ok(_options);
    }

    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        //var response = configuration.GetValue<Jwk>("AppSettings:Jwks");
        return Ok("response");
    }
}