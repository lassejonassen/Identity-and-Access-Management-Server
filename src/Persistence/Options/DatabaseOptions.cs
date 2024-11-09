namespace Persistence.Options;

public sealed class DatabaseOptions
{
    public required string ConnectionString { get; set; } = string.Empty;
    
    public int MaxRetryCount { get; set; } = 3;
    
    public int CommandTimeout { get; set; } = 30;
    
    public bool EnableDetailedErrors { get; set; }
    
    public bool EnableSensitiveDataLogging { get; set; }
}
