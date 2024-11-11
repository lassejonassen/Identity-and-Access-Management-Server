using System.Security.Claims;
using Domain.Modules.Users;

namespace Application.Contracts.Responses;

public class OpenIdConnectLoginResponse
{

    public string UserName { get; set; }
    public string Password { get; set; }
    public string RedirectUri { get; set; }
    public string Code { get; set; }
    public IList<string> RequestedScopes { get; set; }

    public User User { get; set; }
    public ClaimsPrincipal ClaimsPrincipal { get; set; }
    public bool Succeeded { get; set; }
    public string Error { get; set; } = string.Empty;

    public string ErrorDescription { get; set; }
    public bool HasError => !string.IsNullOrEmpty(Error);

}
