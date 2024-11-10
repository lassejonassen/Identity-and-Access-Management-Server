using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.DeviceAuthorization;

public interface IDeviceAuthorizationValidation
{
    Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext);
}