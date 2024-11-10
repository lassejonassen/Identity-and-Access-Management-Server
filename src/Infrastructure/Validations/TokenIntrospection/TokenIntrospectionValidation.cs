using System.Text;
using Abstractions;
using Common.Clients;
using Common.Tokens;
using Domain.Modules.Clients;
using Infrastructure.Enumerations;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.TokenIntrospection;
public class TokenIntrospectionValidation(IHttpContextAccessor httpContextAccessor) 
    : ITokenIntrospectionValidation
{
    private readonly ClientStore _clientStore = new ClientStore();

    public virtual Task<TokenIntrospectionValidationResponse> ValidateAsync(TokenIntrospectionRequest tokenIntrospectionRequest)
    {
        var response = new TokenIntrospectionValidationResponse() { Succeeded = true };
        var context = httpContextAccessor.HttpContext;
        var requestContentType = context.Request.ContentType;

        if (!requestContentType.Equals(Constants.ContentTypeSupported.XwwwFormUrlEncoded))
        {
            response.Succeeded = false;
            response.Error = "Content Type is not supported";
            return Task.FromResult(response);
        }

        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        if (authorizationHeader == null)
        {
            response.Succeeded = false;
            response.Error = "Client is not Authorized";
            return Task.FromResult(response);
        }
        if (!authorizationHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
        {
            response.Succeeded = false;
            response.Error = "Client is not Authorized";
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
            string clinetId = authorizationKeys.Substring(0, authorizationResult);
            string clientSecret = authorizationKeys.Substring(authorizationResult + 1);

            // Here I have to get the client from the Client Store
            CheckClientResult client = VerifyClientById(clinetId, true, clientSecret);
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
