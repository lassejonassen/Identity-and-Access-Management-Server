using Application.Contracts.Clients;
using Domain.Modules.Clients;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Interfaces;

public interface IClientService
{
    Task<CheckClientResult> GetClientByUriAsync(string? clientUrl);

    CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null,
            string grantType = null);

    AudienceValidator ValidateAudienceHandler(IEnumerable<string> audiences, SecurityToken securityToken,
            TokenValidationParameters validationParameters, Client client, string token);

    bool SearchForClientBySecret(string grantType);
}
