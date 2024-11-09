using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Validations.DeviceAuthorization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Abstractions.Validations;

public interface IDeviceAuthorizationValidation
{
    Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext);
}