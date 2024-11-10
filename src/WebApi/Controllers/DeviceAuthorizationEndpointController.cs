using Common.Devices;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/device-authorization-endpoint")]
[ApiController]
public class DeviceAuthorizationEndpointController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDeviceAuthorizationService _deviceAuthorizationService;
    public DeviceAuthorizationEndpointController(IHttpContextAccessor httpContextAccessor,
        IDeviceAuthorizationService deviceAuthorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _deviceAuthorizationService = deviceAuthorizationService;
    }

    [HttpPost("authorization")]
    public async Task<IActionResult> Authorization()
    {
        var result = await _deviceAuthorizationService
            .GenerateDeviceAuthorizationCodeAsync(_httpContextAccessor.HttpContext!);

        if (result is null)
        {
            return BadRequest("Invalid client");
        }

        return Ok(result);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost] 
    public async Task<IActionResult> Post(UserInteractionRequest userInteractionRequest)
    {
        var result = await _deviceAuthorizationService.DeviceFlowUserInteractionAsync(userInteractionRequest.UserCode);

        if (!result)
        {
            return Ok();
        }

        return Redirect("/");
    }
}
