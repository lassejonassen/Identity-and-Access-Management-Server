using CommandLine;

namespace Persistence.DbUp;

public class CommandLineOptions
{
    [Option('c', "connectionString", Required = true, HelpText = "Connection string to the database")]
    public string ConnectionString { get; set; } = string.Empty;
}