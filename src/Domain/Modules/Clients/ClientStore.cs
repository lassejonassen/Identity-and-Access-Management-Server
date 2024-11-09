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

    public IEnumerable<Client> Clients = [];
}
