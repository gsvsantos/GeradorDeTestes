using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class GeradorDeTestesDbContext : DbContext
{
    // Espaço para por as listas. Exemplo: public DbSet<Teste> Testes { get; set; }
    public DbSet<Materia> Materias { get; set; }
    public DbSet<Teste> Testes { get; set; }

    public GeradorDeTestesDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorDeTestesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
