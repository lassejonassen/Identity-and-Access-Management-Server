using Application.Abstractions.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.DeviceAuthorization;

public class DeviceAuthorizationValidation(IClientService clientService) : IDeviceAuthorizationValidation
{
    public Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext) => throw new NotImplementedException();
}
