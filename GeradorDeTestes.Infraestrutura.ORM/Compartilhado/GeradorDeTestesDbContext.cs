using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class GeradorDeTestesDbContext : DbContext, IUnitOfWork
{
    public DbSet<Alternativa> Alternativas { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Materia> Materias { get; set; }
    public DbSet<Questao> Questoes { get; set; }
    public DbSet<Teste> Testes { get; set; }
    public DbSet<TesteMateriaQuantidade> QuantidadesPorMateria { get; set; }

    public GeradorDeTestesDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorDeTestesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public void Commit()
    {
        SaveChanges();
    }

    public void Rollback()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}
