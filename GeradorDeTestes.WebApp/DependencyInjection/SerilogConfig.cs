using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class SerilogConfig
{
    public static void AddSerilogConfig(this IServiceCollection services, ILoggingBuilder logging, IConfiguration configuration)
    {
        string? licenseKey = configuration["NEWRELIC_LICENSE_KEY"];

        if (string.IsNullOrWhiteSpace(licenseKey))
            throw new Exception("A variável NEWRELIC_LICENSE_KEY não foi fornecida.");

        string caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoArquivo = Path.Combine(caminhoAppData, "eAgenda", "erro.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), caminhoArquivo, LogEventLevel.Error)
            .WriteTo.NewRelicLogs(
                endpointUrl: "https://log-api.newrelic.com/log/v1",
                applicationName: "gerador-de-testes-app",
                licenseKey: licenseKey
            )
            .CreateLogger();

        logging.ClearProviders();

        services.AddSerilog(Log.Logger);
    }
}
// 