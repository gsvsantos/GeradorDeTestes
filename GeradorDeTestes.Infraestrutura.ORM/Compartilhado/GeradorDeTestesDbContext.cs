using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class GeradorDeTestesDbContext : DbContext
{
    public GeradorDeTestesDbContext(DbContextOptions options) : base(options) { }
    // Espaço para por as listas. Exemplo: public DbSet<Teste> Testes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorDeTestesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
