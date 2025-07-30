using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Testes.Integracao.ModuloDisciplina;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GeradorDeTestes.Testes.Integracao.Compartilhado;
public class TesteDbContextFactory
{
    public static GeradorDeTestesDbContext CriarDbContext()
    {
        IConfiguration configuracao = CriarConfiguracao();

        DbContextOptions<GeradorDeTestesDbContext> options = new DbContextOptionsBuilder<GeradorDeTestesDbContext>()
            .UseNpgsql(configuracao["SQL_CONNECTION_STRING"])
            .Options;

        GeradorDeTestesDbContext dbContext = new(options);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private static IConfiguration CriarConfiguracao()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets(typeof(RepositorioDisciplinaORMTestes).Assembly)
            .Build();
    }
}
