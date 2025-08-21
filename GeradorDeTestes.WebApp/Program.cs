using GeradorDeTestes.Aplicacao.ModuloAutenticacao;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Aplicacao.ModuloQuestao;
using GeradorDeTestes.Aplicacao.ModuloTeste;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
using GeradorDeTestes.WebApp.ActionFilters;
using GeradorDeTestes.WebApp.DependencyInjection;
using GeradorDeTestes.WebApp.ORM;

namespace GeradorDeTestes.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        builder.Services.AddControllersWithViews((options) =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
            options.Filters.Add<LogarAcaoAttribute>();
        });

        builder.Services.AddScoped<IRepositorioDisciplina, RepositorioDisciplinaORM>();
        builder.Services.AddScoped<IRepositorioMateria, RepositorioMateriaORM>();
        builder.Services.AddScoped<IRepositorioQuestao, RepositorioQuestaoORM>();
        builder.Services.AddScoped<IRepositorioTeste, RepositorioTesteORM>();

        builder.Services.AddScoped<AutenticacaoAppService>();
        builder.Services.AddScoped<DisciplinaAppService>();
        builder.Services.AddScoped<MateriaAppService>();
        builder.Services.AddScoped<QuestaoAppService>();
        builder.Services.AddScoped<TesteAppService>();

        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddIndentityProviderConfig();
        builder.Services.AddCookieAuthenticationConfig();
        builder.Services.AddSerilogConfig(builder.Logging, builder.Configuration);
        builder.Services.AddQuestPDFConfig();
        builder.Services.AddGeminiChatConfig();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<GeradorDeTestesDbContext>();

        WebApplication app = builder.Build();

        bool applyMigrations = builder.Configuration.GetValue<bool>("ApplyMigrations");

        if (applyMigrations)
        {
            app.ApplyMigrations();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/erro");
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.MapHealthChecks("/health");
        app.MapDefaultControllerRoute();

        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}
