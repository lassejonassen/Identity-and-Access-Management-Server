using Common.Devices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Abstractions.Services;

public interface IDeviceAuthorizationService
{
    Task<DeviceAuthorizationResponse> GenerateDeviceAuthorizationCodeAsync(HttpContext httpContext);
    Task<bool> DeviceFlowUserInteractionAsync(string userCode);
}
