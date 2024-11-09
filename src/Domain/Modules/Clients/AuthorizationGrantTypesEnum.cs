using Abstractions;

namespace Domain.Modules.Clients;

public class AuthorizationGrantTypesEnum : Enumeration<AuthorizationGrantTypesEnum>
{
    public static readonly AuthorizationGrantTypesEnum Code = new(1, "code");
    public static readonly AuthorizationGrantTypesEnum ClientCredentials = new(2, "client_credentials");
    public static readonly AuthorizationGrantTypesEnum RefreshToken = new(3, "refresh_token");
    public static readonly AuthorizationGrantTypesEnum AuthorizationCode = new(1, "authorization_code");
    public static readonly AuthorizationGrantTypesEnum DeviceCode = new(1, "urn:ietf:params:oauth:grant-type:device_code");

    public AuthorizationGrantTypesEnum(int value, string name) : base(value, name)
    {
    }
}
