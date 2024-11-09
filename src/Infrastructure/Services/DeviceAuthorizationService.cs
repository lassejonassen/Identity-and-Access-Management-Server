using Common.Devices;
using Domain.Modules.Devices;
using Infrastructure.Abstractions.Services;
using Infrastructure.Abstractions.Validations;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class DeviceAuthorizationService : IDeviceAuthorizationService
{
    private readonly IDeviceAuthorizationValidation _validation;
    private readonly OAuthServerOptions _options;
    private readonly IDeviceFlowRepository _deviceFlowRepository;


    public async Task<bool> DeviceFlowUserInteractionAsync(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
        {
            return false;
        }

        var data = await _deviceFlowRepository.GetByUserCodeAsync(userCode);

        if(data is not null)
        {
            if (data.ExpireIn > DateTime.Now)
            {
                var result = await _deviceFlowRepository.UserInteractionComplete(data);

                if (result > 0)
                {
                    return true;
                }

                return false;
            }

            return false;
        } else
        {
            return false;
        }
    }

    public async Task<DeviceAuthorizationResponse?> GenerateDeviceAuthorizationCodeAsync(HttpContext httpContext)
    {
        var validationResult = await _validation.ValidateAsync(httpContext);

        if (!validationResult.Succeeded)
        {
            return null;
        }

        var response = new DeviceAuthorizationResponse
        {
            UserCode = GenerateUserCode(),
            DeviceCode = GenerateDeviceCode(),
            VerificationUri = _options.IDPUri + "/device",
            ExpiresIn = 300, // user code and device code are valid for 5 minutes.
            Interval = _options.DeviceFlowInterval,

        };

        // Store the responst in the back store (sql server in my case)
        var deviceflow = new DeviceFlow
        {
            ClientId = validationResult.Client.ClientId.ToString(),
            CreatedDate = DateTime.Now,
            UserCode = response.UserCode,
            DeviceCode = response.DeviceCode,
            ExpireIn = DateTime.Now.AddSeconds(response.ExpiresIn),
            UserInteractionComplete = false,
            SessionId = httpContext.Session.Id,
            RequestedScope = validationResult.RequestedScope != null ? validationResult.RequestedScope : default,

        };

        await _deviceFlowRepository.AddAsync(deviceflow);

        return response;
    }

    // The main answer by Dan Rigby at https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    // But I enhance the initiated of the rendom class to create a new thread for every request.
    private string GenerateUserCode(int? length = null)
    {
        length ??= 8;
        // Remove small letters and (Zero / One ) and I and O
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var lengthCount = new char[length.Value];
        var random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        for (int i = 0; i < lengthCount.Length; i++)
        {
            lengthCount[i] = chars[random.Value!.Next(chars.Length)];
        }

        var result = new String(lengthCount);
        return result;
    }

    static string GenerateDeviceCode(int? length = null)
    {
        length ??= 40;
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var lengthCount = new char[length.Value];
        var random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        for (int i = 0; i < lengthCount.Length; i++)
        {
            lengthCount[i] = chars[random.Value!.Next(chars.Length)];
        }

        var result = new string(lengthCount);

        return result;
    }
}
