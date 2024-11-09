using Abstractions;

namespace Domain.Modules.Clients;

public sealed class Client : Entity, IAuditableEntity
{
    private const int ClientNameMaxLength = 50;

    protected Client() { }

    private Client(Guid id, Guid correlationId,
        string clientName, Guid clientId, Guid clientSecret, IList<string> grantTypes, IList<string> allowedScopes, string clientUri, string redirectUri)
        : base(id, correlationId)
    {
        ClientName = clientName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        GrantTypes = grantTypes;
        IsActive = false;
        AllowedScopes = allowedScopes;
        ClientUri = clientUri;
        RedirectUri = redirectUri;
        UsePkce = false;
    }

    public string ClientName { get; private set; }

    public Guid ClientId { get; private set; }
    public Guid ClientSecret { get; private set; }
    public IList<string> GrantTypes { get; private set; } = [];

    /// <summary>
    /// By default the client is not active.
    /// </summary>
    public bool IsActive { get; private set; }
    public ICollection<string> AllowedScopes { get; private set; } = [];

    public string ClientUri { get; private set; }
    public string RedirectUri { get; private set; }

    /// <summary>
    /// By default UsePkce is false.
    /// </summary>
    public bool UsePkce { get; private set; }

    /// <summary>
    /// Get or set the name of the clients/protected resources that are related to this client.
    /// </summary>
    public ICollection<string> AllowedProtectedResources { get; private set; } = [];

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public static Result<Client> Create(Guid correlationId, string clientName, string clientUri, string grantType, string redirectUri)
    {
        if (string.IsNullOrWhiteSpace(clientName))
        {
            return Result.Failure<Client>(ClientErrors.NameIsNullOrEmpty);
        }

        if (clientName.Length > ClientNameMaxLength)
        {
            return Result.Failure<Client>(ClientErrors.NameIsLongerThanAllowed);
        }

        if (!string.IsNullOrWhiteSpace(clientUri))
        {
            var valid = Uri.TryCreate(clientUri, UriKind.Absolute, out var uri);

            if (!valid)
            {
                return Result.Failure<Client>(ClientErrors.ClientUriInvalidFormat);
            }
        }

        if (!string.IsNullOrWhiteSpace(redirectUri))
        {
            var valid = Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri);

            if (!valid)
            {
                return Result.Failure<Client>(ClientErrors.RedirectUriInvalidFormat);
            }
        }

        var grantTypes = grantType switch
        {
            "Code" => GrantType.Code,
            "ClientCredentials" => GrantType.ClientCredentials,
            "RefreshToken" => GrantType.RefreshToken,
            "CodeAndCredentials" => GrantType.CodeAndCredentials,
            "DeviceCode" => GrantType.DeviceCode,
            _ => throw new ArgumentException("Invalid GrantType")
        };


        Guid clientId = Guid.NewGuid();
        Guid clientSecret = Guid.NewGuid();

        var allowedScopes = new List<string> { "openid", "profile", "email" };

        var client = new Client(Guid.NewGuid(), correlationId, clientName, clientId, clientSecret, grantTypes, allowedScopes, clientUri, redirectUri);

        return Result.Success(client);
    }
}
