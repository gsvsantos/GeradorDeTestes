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

    public async Task EncerrarAsync()
    {
        await container.StopAsync();
        await container.DisposeAsync();
    }

    public GeradorDeTestesDbContext CriarDbContext()
    {
        DbContextOptions<GeradorDeTestesDbContext> options = new DbContextOptionsBuilder<GeradorDeTestesDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;

        GeradorDeTestesDbContext dbContext = new(options);

        return dbContext;
    }
}
