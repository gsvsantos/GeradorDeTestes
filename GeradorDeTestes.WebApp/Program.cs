using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
using GeradorDeTestes.WebApp.ActionFilters;
using GeradorDeTestes.WebApp.DependencyInjection;
using GeradorDeTestes.WebApp.ORM;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GeradorDeTestes.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews((options) =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
            options.Filters.Add<LogarAcaoAttribute>();
        });
        builder.Services.AddScoped<IDbConnection>(_ =>
        {
            string? connectionString = builder.Configuration["SQL_CONNECTION_STRING"];

            return new SqlConnection(connectionString);
        });
        // Exemplo builder.Services.AddScoped<IRepositorioTeste, RepositorioTesteORM>();

        builder.Services.AddScoped<IRepositorioDisciplina, RepositorioDisciplinaORM>();
        builder.Services.AddScoped<IRepositorioQuestao, RepositorioQuestaoORM>();
        builder.Services.AddScoped<IRepositorioTeste, RepositorioTesteORM>();
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
