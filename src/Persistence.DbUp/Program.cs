using CommandLine;
using Microsoft.EntityFrameworkCore;
using Persistence.DbUp;
using Serilog;

var options = Parser.Default.ParseArguments<CommandLineOptions>(args);
var connectionString = options.Value.ConnectionString;

var contextFactory = new DbContextFactory(connectionString);

using (var context = contextFactory.CreateDbContext(args))
{
    context.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));

    if (context.Database.GetPendingMigrations().Any())
    {
        try
        {
            Log.Logger.Information("Starting pending migrations");

            context.Database.Migrate();

            Log.Logger.Information("Finished pending migrations");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }
    }
    else
    {
        Log.Logger.Information("No pending migrations to apply");
    }
}
