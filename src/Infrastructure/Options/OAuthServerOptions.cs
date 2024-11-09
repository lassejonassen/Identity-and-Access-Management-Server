namespace Infrastructure.Options;

public class OAuthServerOptions
{
    /// <summary>
    /// This indicate which provider our identity provider will use
    /// InMemoey Or BackStore
    /// </summary>
    public string Provider { get; set; }

    /// <summary>
    /// If is not availabe so, no user can login or register
    /// This will shout down the application domain
    /// </summary>
    public bool IsAvaliable { get; set; }

    /// <summary>
    /// This is the uri of the identity provider
    /// </summary>
    public string IDPUri { get; set; }

    /// <summary>
    /// Get or set the device flow interval value in seconds to let client wait before
    /// calling the Token endpoint repeatedly. <c>Default value is 5 seconds</c>
    /// </summary>
    public int DeviceFlowInterval { get; set; } = 5;

}
