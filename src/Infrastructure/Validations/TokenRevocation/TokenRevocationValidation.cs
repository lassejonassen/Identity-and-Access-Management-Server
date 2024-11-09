using System.Text;
using Common.Clients;
using Domain.Modules.Clients;
using Infrastructure.Abstractions.Validations;
using Infrastructure.Enumerations;
using Microsoft.AspNetCore.Http;
using Persistence.DbContexts;

namespace Infrastructure.Validations.TokenRevocation;
public class TokenRevocationValidation : ITokenRevocationValidation
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ClientStore _clientStore = new();

    public TokenRevocationValidation(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual Task<TokenRevocationValidationResponse> ValidateAsync(HttpContext context)
    {
        var response = new TokenRevocationValidationResponse { Succeeded = true };

        var authorizationHeader = context.Request.Headers["Authorization"].ToString();

        if (authorizationHeader is null)
        {
            response.Succeeded = false;
            response.Error = "Client is not authorized";
            return Task.FromResult(response);
        }

        if (!authorizationHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
        {
            response.Succeeded = false;
            response.Error = "Client is not authorized";
            return Task.FromResult(response);
        }

        try
        {
            string parameters = authorizationHeader.Substring("Basic ".Length);
            string authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));

            int authorizationResult = authorizationKeys.IndexOf(':');
            if (authorizationResult == -1)
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return Task.FromResult(response);
            }

            string clientId = authorizationKeys.Substring(0, authorizationResult);
            string clientSecret = authorizationKeys.Substring(authorizationResult + 1);

            var client = VerifyClientById(clientId, true, clientSecret);

            if (!client.IsSuccess)
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return Task.FromResult(response);
            }

            response.Client = client.Client;
        }
        catch (Exception ex)
        {
            _ = ex;
            response.Succeeded = false;
            response.Error = "Client is not Authorized";
            return Task.FromResult(response);
        }


        return Task.FromResult(response);
    }

    private CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = "")
    {
        CheckClientResult result = new() { IsSuccess = false };

        // TODO 1 LASJN: Check
        bool valid = Guid.TryParse(clientId, out Guid parseClientId);

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            var client = _clientStore.Clients.FirstOrDefault(x => x.ClientId == clientId);
            //var client = _clientStore.Clients.FirstOrDefault(x => x.ClientId == parseClientId);

            if (client is not null)
            {
                var clientGrantTypes = client.GrantTypes;
                bool isClientAllowed = clientGrantTypes.Contains(AuthorizationGrantTypesEnum.ClientCredentials.Name);
                if (!isClientAllowed)
                {
                    result.Error = ErrorTypeEnum.InvalidClient.Name;
                    return result;
                }

                if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                {
                    // TODO 1 LASJN: Check
                    valid = Guid.TryParse(clientSecret, out Guid parseClientSecret);
                    bool hasSamesecretId = client.ClientSecret.Equals(parseClientSecret);

                    if (!hasSamesecretId)
                    {
                        result.Error = ErrorTypeEnum.InvalidClient.Name;
                        return result;
                    }
                }

                if (client.IsActive)
                {
                    result.IsSuccess = true;
                    result.Client = client;
                    return result;
                } else
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
