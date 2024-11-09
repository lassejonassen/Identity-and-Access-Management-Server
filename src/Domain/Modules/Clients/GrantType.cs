namespace Domain.Modules.Clients;

public class GrantType
{
    public static IList<string> Code
        => [AuthorizationGrantTypesEnum.Code.Name];

    public static IList<string> ClientCredentials
        => [AuthorizationGrantTypesEnum.ClientCredentials.Name];

    public static IList<string> RefreshToken
        => [AuthorizationGrantTypesEnum.RefreshToken.Name];

    public static IList<string> CodeAndCredentials
        => [AuthorizationGrantTypesEnum.Code.Name, 
            AuthorizationGrantTypesEnum.ClientCredentials.Name];

    public static IList<string> DeviceCode
        => [AuthorizationGrantTypesEnum.DeviceCode.Name];
}
