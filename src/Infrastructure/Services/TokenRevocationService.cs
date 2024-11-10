using Abstractions;
using Common.Tokens;
using Domain.Modules.OAuth;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;
public class TokenRevocationService(IOAuthTokenRepository oAuthTokenRepository) 
    : ITokenRevocationService
{
    public async Task<TokenRecovationResponse> RevokeTokenAsync(HttpContext httpContext, string clientId)
    {
        var response = new TokenRecovationResponse() { Succeeded = true };

        if (httpContext.Request.ContentType != Constants.ContentTypeSupported.XwwwFormUrlEncoded)
        {
            response.Succeeded = false;
            response.Error = "Not supported content type";
        }

        string token = httpContext.Request.Form["token"];
        String tokenTypeHint = httpContext.Request.Form["token_type_hint"];

        var oauthToken = await oAuthTokenRepository.GetTokenAsync(token, tokenTypeHint, clientId);

        if (oauthToken is not null)
        {
            await oAuthTokenRepository.RevokeToken(oauthToken);
        }

        return response;
    }
}
