using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.IA.Gemini;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class GeminiChatConfig
{
    public static void AddGeminiChatConfig(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IGeradorQuestoes, GeradorQuestoesGemini>();
    }
}
