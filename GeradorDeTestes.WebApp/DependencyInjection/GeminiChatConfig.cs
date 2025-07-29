using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.IA.Gemini.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.IA.Gemini.ModuloQuestao;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class GeminiChatConfig
{
    public static void AddGeminiChatConfig(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IGeradorQuestoes, GeradorQuestoesGemini>();
        services.AddScoped<IGeradorDisciplinas, GeradorDisciplinaGemini>();
    }
}
