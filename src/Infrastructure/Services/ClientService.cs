using System.Text;
using Abstractions;
using Application.Abstractions.Services;
using Common.Clients;
using Domain.Modules.Clients;
using Infrastructure.Enumerations;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Persistence.DbContexts;

namespace Infrastructure.Services;

public sealed class ClientService : IClientService
{
    private readonly ClientStore _clientStore = new ClientStore();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;

    public ClientService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }


    public Task<CheckClientResult> GetClientByUriAsync(string? clientUrl)
    {
        var client = _clientStore.Clients.FirstOrDefault(x => x.ClientUri == clientUrl);

        var response = new CheckClientResult
        {
            Client = client,
            IsSuccess = client != null
        };

        return Task.FromResult(response);
    }

    public bool SearchForClientBySecret(string grantType)
    {
        if (grantType == AuthorizationGrantTypesEnum.ClientCredentials.Name ||
            grantType == AuthorizationGrantTypesEnum.RefreshToken.Name ||
            grantType == AuthorizationGrantTypesEnum.ClientCredentials.Name)
            return true;

        return false;
    }

    public AudienceValidator ValidateAudienceHandler(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters, Client client, string token)
    {
        Func<IEnumerable<string>, SecurityToken, TokenValidationParameters, bool> handler = (audiences, securityToken, validationParameters) =>
        {
            // Check the Token the Back Store.
            var tokenInDb = _dbContext.OAuthTokens.FirstOrDefault(x => x.Token == token);
            if (tokenInDb == null)
                return false;

            if (tokenInDb.Revoked)
                return false;

            return true;
        };
        return new AudienceValidator(handler);
    }

    public CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = "", string grantType = "")
    {
        CheckClientResult result = new() { IsSuccess = false };

        if (!string.IsNullOrWhiteSpace(grantType) &&
                grantType == AuthorizationGrantTypesEnum.ClientCredentials.Name)
        {
            var data = _httpContextAccessor.HttpContext;
            var authHeader = data?.Request.Headers["Authorization"].ToString();

            if (authHeader == null)
            {
                return result;

            }

            if (!authHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
            {
                return result;

            }

            var parameters = authHeader.Substring("Basic ".Length);
            var authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));

            var authorizationResult = authorizationKeys.IndexOf(':');
            if (authorizationResult == -1)
                return result;
            clientId = authorizationKeys.Substring(0, authorizationResult);
            clientSecret = authorizationKeys.Substring(authorizationResult + 1);

        }

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            var client = _clientStore
                .Clients
                .Where(x =>
                x.ClientId.ToString().Equals(clientId, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (client != null)
            {
                if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                {
                    bool hasSamesecretId = client.ClientSecret.ToString().Equals(clientSecret, StringComparison.InvariantCulture);
                    if (!hasSamesecretId)
                    {
                        result.Error = ErrorTypeEnum.InvalidClient.Name;
                        return result;
                    }
                }
                // check if client is enabled or not

                if (client.IsActive)
                {
                    result.IsSuccess = true;
                    result.Client = client;

                    return result;
                }
                else
                {
                    result.ErrorDescription = ErrorTypeEnum.UnauthorizedClient.Name;
                    return result;
                }
            }
        }


        result.ErrorDescription = ErrorTypeEnum.AccessDenied.Name;
        return result;
    }
}
