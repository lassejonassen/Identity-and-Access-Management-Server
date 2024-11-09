using Serilog;
using Serilog.Settings.Configuration;

namespace WebApi.Extensions.Logging;

public static class WebApplicationBuilderExtensions
{
    private const string SectionName = "AppSettings:Logging:Serilog";

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((ctx, cfg) =>
        {
            var options = new ConfigurationReaderOptions { SectionName = SectionName };
            cfg.ReadFrom.Configuration(ctx.Configuration, options); 
        });

        return builder;
    }
}
