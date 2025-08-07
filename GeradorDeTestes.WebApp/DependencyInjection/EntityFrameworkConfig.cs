using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class EntityFrameworkConfig
{
    public static void AddEntityFrameworkConfig(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration["SQL_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("A variável SQL_CONNECTION_STRING não foi fornecida.");

        services.AddDbContext<IUnitOfWork, GeradorDeTestesDbContext>(options =>
        options.UseNpgsql(connectionString, (opt) => opt.EnableRetryOnFailure(3)));
    }
}
