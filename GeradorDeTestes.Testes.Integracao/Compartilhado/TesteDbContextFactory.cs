using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace GeradorDeTestes.Testes.Integracao.Compartilhado;
public class TesteDbContextFactory
{
    private readonly PostgreSqlContainer container;

    public TesteDbContextFactory()
    {
        container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("testes-geradordetestes-postgres")
            .WithDatabase("GeradorDeTestesDbTest")
            .WithCleanUp(true)
            .Build();
    }
    public async Task InicializarAsync()
    {
        await container.StartAsync();
    }

    public GeradorDeTestesDbContext CriarDbContext()
    {
        DbContextOptions<GeradorDeTestesDbContext> options = new DbContextOptionsBuilder<GeradorDeTestesDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;

        GeradorDeTestesDbContext dbContext = new(options);

        ConfigurarDbContext(dbContext);

        return dbContext;
    }

    public async Task EncerrarAsync()
    {
        await container.DisposeAsync();
    }

    private static void ConfigurarDbContext(GeradorDeTestesDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Testes.RemoveRange(dbContext.Testes);
        dbContext.Questoes.RemoveRange(dbContext.Questoes);
        dbContext.Materias.RemoveRange(dbContext.Materias);
        dbContext.Disciplinas.RemoveRange(dbContext.Disciplinas);

        dbContext.SaveChanges();
    }
}
