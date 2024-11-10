using Common.Devices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Interfaces;

public interface IDeviceAuthorizationService
{
    Task<DeviceAuthorizationResponse?> GenerateDeviceAuthorizationCodeAsync(HttpContext httpContext);
    Task<bool> DeviceFlowUserInteractionAsync(string userCode);
}
