using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class EntityFrameworkConfig
{
    public static void AddEntityFrameworkConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IUnitOfWork, GeradorDeTestesDbContext>(options =>
        options.UseNpgsql(configuration["SQL_CONNECTION_STRING"],
        (opt) => opt.EnableRetryOnFailure(3)));
    }
}
