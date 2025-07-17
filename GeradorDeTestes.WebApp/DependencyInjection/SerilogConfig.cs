using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class SerilogConfig
{
    public static void AddSerilogConfig(this IServiceCollection services, ILoggingBuilder logging)
    {
        string caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoArquivo = Path.Combine(caminhoAppData, "eAgenda", "erro.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), caminhoArquivo, LogEventLevel.Error)
            .CreateLogger();

        logging.ClearProviders();

        services.AddSerilog(Log.Logger);
    }
}
