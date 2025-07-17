using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.WebApp.ORM;

public static class DatabaseOperations
{
    public static void ApplyMigrations(this IHost host)
    {
        using IServiceScope? scope = host.Services.CreateScope();
        GeradorDeTestesDbContext? db = scope.ServiceProvider.GetRequiredService<GeradorDeTestesDbContext>();

        db.Database.Migrate();
    }
}
