using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public static class TesteDbContextFactory
{
    public static GeradorDeTestesDbContext CriarDbContext(string connectionString)
    {
        DbContextOptions<GeradorDeTestesDbContext> options = new DbContextOptionsBuilder<GeradorDeTestesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new(options);
    }
}
