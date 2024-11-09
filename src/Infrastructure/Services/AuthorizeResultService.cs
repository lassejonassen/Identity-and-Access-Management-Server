using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Abstractions;
using Application.Abstractions.Services;
using Common;
using Domain.Modules.Clients;
using Domain.Modules.Devices;
using Domain.Modules.OAuth;
using Infrastructure.Abstractions.Services;
using Infrastructure.Enumerations;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;
public class AuthorizeResultService : IAuthorizeResultService
{
    private readonly ICodeStoreService _codeStoreService;
    private readonly IClientService _clientService;
    private readonly OAuthServerOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOAuthTokenRepository _oAuthTokenRepository;
    private readonly IDeviceFlowRepository _deviceFlowsRepository;

    public AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest)
    {
        AuthorizeResponse response = new AuthorizeResponse();

        if (httpContextAccessor == null)
        {
            response.Error = ErrorTypeEnum.ServerError.Name;
            return response;
        }

        var client = _clientService.VerifyClientById(authorizationRequest.client_id);
        if (!client.IsSuccess)
        {
            response.Error = client.ErrorDescription;
            return response;
        }

        if (string.IsNullOrEmpty(authorizationRequest.response_type) || authorizationRequest.response_type != "code")
        {
            response.Error = ErrorTypeEnum.InvalidRequest.Name;
            response.ErrorDescription = "response type is required or is not valid";
            return response;
        }

        if (!authorizationRequest.redirect_uri.IsRedirectUriStartWithHttps() && !httpContextAccessor.HttpContext.Request.IsHttps)
        {
            response.Error = ErrorTypeEnum.InvalidRequest.Name;
            response.ErrorDescription = "redirect url is not secure, MUST be TLS";
            return response;
        }

        if (client.Client.UsePkce && string.IsNullOrWhiteSpace(authorizationRequest.code_challenge))
        {
            response.Error = ErrorTypeEnum.InvalidRequest.Name;
            response.ErrorDescription = "code challenge required";
            return response;
        }

        bool redirectUriIsMatched = client.Client.RedirectUri.Equals(authorizationRequest.redirect_uri, StringComparison.OrdinalIgnoreCase);
        if (!redirectUriIsMatched)
        {
            response.Error = ErrorTypeEnum.InvalidRequest.Name;
            response.ErrorDescription = "redirect uri is not matched the one in the client store";
            return response;
        }
        // check the scope in the client store with the
        // one that is comming from the request MUST be matched at leaset one

        var scopes = authorizationRequest.scope.Split(' ');

        var clientScopes = from m in client.Client.AllowedScopes
                           where scopes.Contains(m)
                           select m;

        if (!clientScopes.Any())
        {
            response.Error = ErrorTypeEnum.InvalidScope.Name;
            response.ErrorDescription = "scopes are invalids";
            return response;
        }

        string nonce = authorizationRequest.nonce;

        // Verify that a scope parameter is present and contains the openid scope value.
        // (If no openid scope value is present,
        // the request may still be a valid OAuth 2.0 request, but is not an OpenID Connect request.)

        var authoCode = new AuthorizationCode
        {
            ClientId = authorizationRequest.client_id,
            RedirectUri = authorizationRequest.redirect_uri,
            RequestedScopes = clientScopes.ToList(),
            Nonce = nonce,
            CodeChallenge = authorizationRequest.code_challenge,
            CodeChallengeMethod = authorizationRequest.code_challenge_method,
            CreationTime = DateTime.UtcNow,
            Subject = httpContextAccessor.HttpContext.User //as ClaimsPrincipal

        };

        string code = _codeStoreService.GenerateAuthorizationCode(authoCode);
        if (code == null)
        {
            response.Error = ErrorTypeEnum.TemporarilyUnavailable.Name;
            return response;
        }

        response.RedirectUri = client.Client.RedirectUri + "?response_type=code" + "&state=" + authorizationRequest.state;
        response.Code = code;
        response.State = authorizationRequest.state;
        response.RequestedScopes = clientScopes.ToList();

        return response;
    }

    public async Task<TokenResponse> GenerateToken(TokenRequest tokenRequest)
    {
        // TODO: this method needs a refactor, and need to call generate token validation method  

        var result = new TokenResponse();
        var searchBySecret = _clientService.SearchForClientBySecret(tokenRequest.grant_type);

        var checkClientResult = _clientService.VerifyClientById(tokenRequest.client_id, searchBySecret, tokenRequest.client_secret, tokenRequest.grant_type);
        if (!checkClientResult.IsSuccess)
        {
            return new TokenResponse { Error = checkClientResult.Error, ErrorDescription = checkClientResult.ErrorDescription };
        }

        // Check first if the authorization_grant is DeviceCode...
        // then generate the jwt access token and store it to back store
        if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.DeviceCode.Name)
        {
            var clientHasDeviceCodeGrant = checkClientResult.Client.GrantTypes.Contains(tokenRequest.grant_type);
            if (!clientHasDeviceCodeGrant)
            {
                result.Error = ErrorTypeEnum.InvalidGrant.Name;
                return result;
            }
            var deviceCode = await _deviceFlowsRepository.GetAsync(tokenRequest.device_code, tokenRequest.client_id);

            if (deviceCode == null)
            {
                result.Error = ErrorTypeEnum.InvalidRequest.Name;
                result.ErrorDescription = "Please check the device code";
                return result;
            }

            if (deviceCode.UserInteractionComplete == false)
            {
                result.Error = ErrorTypeEnum.WaitForUserInteraction.Name;
                return result;
            }


            if (deviceCode.ExpireIn < DateTime.Now)
            {
                result.Error = ErrorTypeEnum.InvalidRequest.Name;
                result.ErrorDescription = "The device code is expired";
                return result;
            }

            var requestedScope = deviceCode.RequestedScope.Split(' ');
            IEnumerable<string> scopes = checkClientResult.Client.AllowedScopes.Intersect(requestedScope);


            var deviceflowAccessTokenResult = GenerateJsonWebToken(scopes, Constants.TokenTypes.JWTAccessToken, checkClientResult.Client, null);
            await SaveJsonWebTokenAsync(checkClientResult.Client.ClientId.ToString(), deviceflowAccessTokenResult.AccessToken, deviceflowAccessTokenResult.ExpirationDate);

            result.access_token = deviceflowAccessTokenResult.AccessToken;
            result.id_token = null;
            return result;
        }

        // Check first if the authorization_grant is client_credentials...
        // then generate the jwt access token and store it to back store.
        if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.ClientCredentials.Name)
        {
            var clientHasClientCredentialsGrant = checkClientResult.Client.GrantTypes.Contains(tokenRequest.grant_type);
            if (!clientHasClientCredentialsGrant)
            {
                result.Error = ErrorTypeEnum.InvalidGrant.Name;
                return result;
            }
            IEnumerable<string> scopes = checkClientResult.Client.AllowedScopes.Intersect(tokenRequest.scope);

            var clientCredentialAccessTokenResult = GenerateJsonWebToken(scopes, Constants.TokenTypes.JWTAccessToken, checkClientResult.Client, null);
            await SaveJsonWebTokenAsync(checkClientResult.Client.ClientId.ToString(), clientCredentialAccessTokenResult.AccessToken, clientCredentialAccessTokenResult.ExpirationDate);

            result.access_token = clientCredentialAccessTokenResult.AccessToken;
            result.id_token = null; // I have to use data shaping here to remove this property or I can customize the return data in the json result, but for now null is ok.
            result.code = tokenRequest.code;
            return result;
        }

        // check code from the Concurrent Dictionary
        var clientCodeChecker = _codeStoreService.GetClientDataByCode(tokenRequest.code);
        if (clientCodeChecker == null)
            return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.Name };

        // check if the current client who is one made this authentication request

        if (tokenRequest.client_id != clientCodeChecker.ClientId)
            return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.Name };

        // TODO: 
        // also I have to check the rediret uri 

        if (checkClientResult.Client.UsePkce)
        {
            var pkceResult = CodeVerifierIsSendByTheClientThatReceivedTheCode(tokenRequest.code_verifier,
                clientCodeChecker.CodeChallenge, clientCodeChecker.CodeChallengeMethod);

            if (!pkceResult)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.Name };
        }

        string id_token = string.Empty;
        string userId = null;
        if (clientCodeChecker.IsOpenId)
        {
            if (!clientCodeChecker.Subject.Identity.IsAuthenticated)
                // I have to inform the caller to redirect the user to the login page
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.Name };

            var currentUserName = clientCodeChecker.Subject.Identity.Name;

            userId = clientCodeChecker.Subject.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentUserName))
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.Name };

            // Generate Identity Token
            int iat = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string[] amrs = new string[] { "pwd" };

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

            string publicPrivateKey = File.ReadAllText("PublicPrivateKey.xml");
            provider.FromXmlString(publicPrivateKey);

            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);

            var claims = new List<Claim>()
                    {
                        new Claim("sub", userId, ClaimValueTypes.String),
                        new Claim("given_name", currentUserName, ClaimValueTypes.String),
                        new Claim("iat", iat.ToString(), ClaimValueTypes.Integer), // time stamp
                        new Claim("nonce", clientCodeChecker.Nonce, ClaimValueTypes.String)
                    };
            foreach (var amr in amrs)
                claims.Add(new Claim("amr", amr));// authentication

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken(_options.IDPUri, checkClientResult.Client.ClientId.ToString(), claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse("50")), signingCredentials: new
                SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256));
            id_token = handler.WriteToken(token);

            var idptoken = new OAuthToken
            {
                ClientId = checkClientResult.Client.ClientId.ToString(),
                CreationDate = DateTime.Now,
                ReferenceId = Guid.NewGuid().ToString(),
                Status = Constants.Statuses.Valid,
                Token = id_token,
                TokenType = Constants.TokenTypes.JWTIdentityToken,
                ExpirationDate = token.ValidTo,
                SubjectId = userId,
                Revoked = false
            };

            await _oAuthTokenRepository.AddAsync(idptoken);
        }

        var scopesinJWtAccessToken = from m in clientCodeChecker.RequestedScopes.ToList()
                                     where !OAuth2ServerHelpers.OpenIdConnectScopes.Contains(m)
                                     select m;

        var accessTokenResult = GenerateJsonWebToken(scopesinJWtAccessToken, Constants.TokenTypes.JWTAccessToken, checkClientResult.Client, userId);
        SaveJsonWebTokenAsync(checkClientResult.Client.ClientId.ToString(), accessTokenResult.AccessToken, accessTokenResult.ExpirationDate);

        // here remove the code from the Concurrent Dictionary
        _codeStoreService.RemoveClientDataByCode(tokenRequest.code);

        result.access_token = accessTokenResult.AccessToken;
        result.id_token = id_token;
        result.code = tokenRequest.code;
        return result;
    }

    private async Task<int> SaveJsonWebTokenAsync(string clientId, string accessToken, DateTime expireIn)
    {
        var atoken = new OAuthToken
        {
            ClientId = clientId,
            CreationDate = DateTime.Now,
            ReferenceId = Guid.NewGuid().ToString(),
            Status = Constants.Statuses.Valid,
            Token = accessToken,
            TokenType = Constants.TokenTypes.JWTAccessToken,
            TokenTypeHint = Constants.TokenTypeHints.AccessToken,
            ExpirationDate = expireIn,
            Revoked = false
        };

        var result = await _oAuthTokenRepository.AddAsync(atoken);
        return result;
    }

    private bool CodeVerifierIsSendByTheClientThatReceivedTheCode(string codeVerifier, string codeChallenge, string codeChallengeMethod)
    {
        var odeVerifireAsByte = Encoding.ASCII.GetBytes(codeVerifier);

        if (codeChallengeMethod == Constants.ChallengeMethod.Plain)
        {
            return codeVerifier.Equals(codeChallenge);
        }

        else if (codeChallengeMethod == Constants.ChallengeMethod.SHA256)
        {

            using var shaS256 = SHA256.Create();
            var computedHashS256 = shaS256.ComputeHash(odeVerifireAsByte);
            var tranformedResultS256 = Base64UrlEncoder.Encode(computedHashS256);

            return tranformedResultS256.Equals(codeChallenge);
        }
        else
        {
            return false;
        }
    }

    public TokenResult GenerateJsonWebToken(IEnumerable<string> scopes, string tokenType, Client client, string sub)
    {
        var result = new TokenResult();

        if (tokenType == Constants.TokenTypes.JWTAccessToken)
        {
            var claims = new List<Claim>
                {
                    new Claim("scope", string.Join(' ', scopes))
                };

            if (!string.IsNullOrEmpty(sub))
            {
                claims.Add(new Claim("sub", sub, ClaimValueTypes.String));
            }

            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider();

            string publicPrivateKey1 = File.ReadAllText("PublicPrivateKey.xml");
            provider1.FromXmlString(publicPrivateKey1);

            RsaSecurityKey rsaSecurityKey1 = new RsaSecurityKey(provider1);
            JwtSecurityTokenHandler handler1 = new JwtSecurityTokenHandler();

            var token1 = new JwtSecurityToken(_options.IDPUri, client.ClientUri, claims, notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse("50")), signingCredentials: new
                SigningCredentials(rsaSecurityKey1, SecurityAlgorithms.RsaSha256));

            string access_token = handler1.WriteToken(token1);

            result.AccessToken = access_token;
            result.TokenType = tokenType;
            result.ExpirationDate = token1.ValidTo;
        }
        else
        {

        }

        return result;
    }
}
