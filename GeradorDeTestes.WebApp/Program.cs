using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Aplicacao.ModuloQuestao;
using GeradorDeTestes.Aplicacao.ModuloTeste;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Dapper.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
using GeradorDeTestes.WebApp.ActionFilters;
using GeradorDeTestes.WebApp.DependencyInjection;
using GeradorDeTestes.WebApp.ORM;
using Npgsql;
using System.Data;

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
        builder.Services.AddScoped<IDbConnection>(_ =>
        {
            string? connectionString = builder.Configuration["SQL_CONNECTION_STRING"];

            return new NpgsqlConnection(connectionString);
        });

        builder.Services.AddScoped<IRepositorioDisciplina, RepositorioDisciplinaDapper>();
        builder.Services.AddScoped<IRepositorioMateria, RepositorioMateriaORM>();
        builder.Services.AddScoped<IRepositorioQuestao, RepositorioQuestaoORM>();
        builder.Services.AddScoped<IRepositorioTeste, RepositorioTesteORM>();
        builder.Services.AddScoped<DisciplinaAppService>();
        builder.Services.AddScoped<MateriaAppService>();
        builder.Services.AddScoped<QuestaoAppService>();
        builder.Services.AddScoped<TesteAppService>();
        builder.Services.AddScoped<GeradorPdfService>();
        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddSerilogConfig(builder.Logging);

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

        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.MapDefaultControllerRoute();

        app.Run();
    }
}
