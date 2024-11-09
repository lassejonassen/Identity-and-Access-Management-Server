using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Application.Abstractions.Services;
using Application.Contracts.Users;
using Infrastructure.Abstractions.Validations;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Infrastructure.Services;

public sealed class UserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBearerTokenUsageTypeValidation _bearerTokenUsageTypeValidation;
    private readonly OAuthServerOptions _options;
    private readonly IClientService _clientService;
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    public UserInfoService(IHttpContextAccessor httpContextAccessor,
        IBearerTokenUsageTypeValidation bearerTokenUsageTypeValidation,
        IOptionsSnapshot<OAuthServerOptions> options,
        IClientService clientService,
        IUserService userService,
        ILogger logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _bearerTokenUsageTypeValidation = bearerTokenUsageTypeValidation;
        _options = options.Value ?? new();
        _clientService = clientService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<UserInfoResponse> GetUserInfoAsync()
    {
        var response = new UserInfoResponse();

        var bearerTokenUsages = await _bearerTokenUsageTypeValidation.ValidateAsync();

        if (!bearerTokenUsages.Succeeded)
        {
            response.Claims = null;
            response.Succeeded = false;
            response.Error = "No bearer token found";
            response.ErrorDescription = "Make sure to add the bearer token to the Authorization header in the request";
        }
        else
        {
            RSACryptoServiceProvider provider = new();
            string publicPrivateKey = File.ReadAllText("PublicPrivateKey.xml");
            provider.FromXmlString(publicPrivateKey);
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(bearerTokenUsages.Token);

            var aud = jwtSecurityToken.Audiences.FirstOrDefault();
            var client = await _clientService.GetClientByUriAsync(aud);

            if (client is null)
            {
                response.Claims = null;
                response.Succeeded = false;
                response.Error = "Client is null";
                response.ErrorDescription = "The specified client does not exist";

                return response;
            }

            if (!client.Client.IsActive)
            {
                response.Claims = null;
                response.Succeeded = false;
                response.Error = "Client is inactive";
                response.ErrorDescription = "The specified client is not active";

                return response;
            }

            TokenValidationParameters tokenValidationParameters = new()
            {
                IssuerSigningKey = rsaSecurityKey,
                ValidAudiences = jwtSecurityToken.Audiences!,
                ValidTypes = ["JWT"],
                ValidateIssuer = true,
                ValidIssuer = _options.IDPUri,
                ValidateAudience = true,
            };

            tokenValidationParameters.AudienceValidator = _clientService.ValidateAudienceHandler(
                jwtSecurityToken.Audiences, jwtSecurityToken,
                tokenValidationParameters, client.Client, bearerTokenUsages.Token);

            try
            {
                var tokenValidationResult = await jwtSecurityTokenHandler.ValidateTokenAsync(bearerTokenUsages.Token, tokenValidationParameters);

                if (tokenValidationResult.IsValid)
                {
                    var payload = jwtSecurityToken.Payload;
                    var userId = payload.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        throw new ArgumentException("UserId is null");
                    }

                    var user = await _userService.GetUserAsync(userId);

                    if (user.IsFailure)
                    {
                        throw new ArgumentException("User is null");
                    }

                    response.Sub = userId;
                    response.EmailVerified = false;
                    response.Email = user.Value.Email!;
                    response.Name = user.Value.Email!;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("There is an exception during fetching the user info, {ex}", ex);
                response.Claims = null;
                response.Succeeded = false;
                response.Error = "invalid_token";
                response.ErrorDescription = "Token is not valid";
            }
        }

        return response;
    }
}
