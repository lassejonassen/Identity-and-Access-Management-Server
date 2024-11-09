using Application.Abstractions.Services;
using Infrastructure.Abstractions.Validations;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.DeviceAuthorization;

public class DeviceAuthorizationValidation(IClientService clientService) : IDeviceAuthorizationValidation
{
    public Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext) => throw new NotImplementedException();
}
