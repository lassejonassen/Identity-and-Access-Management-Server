namespace Domain.Modules.Clients;

public class ClientStore
{
    protected static readonly Lazy<ClientStore> _lazyInstance = new Lazy<ClientStore>(() => new ClientStore());

    /// <summary>
    /// Get the Singleton Instance for this Object.
    /// </summary>
    public static ClientStore Instance
    {
        get
        {
            return _lazyInstance.Value;
        }
    }

    public IEnumerable<Client> Clients = [
            new() {
                ClientName = "MVC_ClientApp_OIDC",
                ClientId = "4285cc0c-c287-4d09-8320-bf7a28f3cbf4",
                ClientSecret = "e79a0552-949f-422a-a7b0-30d249929f28",
                AllowedScopes = ["openid", "profile"],
                IsActive = true,
                ClientUri = "https://localhost:5003",
                RedirectUri = "https://localhost:5003/signin-oidc",
                UsePkce = true,
            }
        ];
}
