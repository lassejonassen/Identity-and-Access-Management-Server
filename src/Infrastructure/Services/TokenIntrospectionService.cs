using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Contracts.Responses;
using Application.Contracts.Tokens;
using Domain.Modules.Clients;
using Domain.Modules.OAuth;
using Infrastructure.Services.Interfaces;
using Infrastructure.Validations.TokenIntrospection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;
public class TokenIntrospectionService : ITokenIntrospectionService
{
    private readonly IOAuthTokenRepository _oAuthTokenRepository;
    private readonly ITokenIntrospectionValidation _tokenIntrospectionValidation;

    public async Task<TokenIntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest tokenIntrospectionRequest)
    {
        var response = new TokenIntrospectionResponse();

        return response;
    }


    private AudienceValidator ValidateAudienceHandler(IEnumerable<string> audiences, SecurityToken securityToken,
            TokenValidationParameters validationParameters, Client client, string token)
    {
        Func<IEnumerable<string>, SecurityToken, TokenValidationParameters, bool> handler = (audiences, securityToken, validationParameters) =>
        {
            // Check the Token the Back Store.
            var tokenInDb = _oAuthTokenRepository.GetTokenAsync(token);
            if (tokenInDb.Result == null)
                return false;

            if (tokenInDb.Result.Revoked)
                return false;

            return true;
        };
        return new AudienceValidator(handler);
    }

    private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        => tokenContent.Claims.ToList();
}
