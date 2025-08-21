using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloAutenticacao;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class GeradorDeTestesDbContext : IdentityDbContext<Usuario, Cargo, Guid>, IUnitOfWork
{
    public DbSet<Alternativa> Alternativas { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Materia> Materias { get; set; }
    public DbSet<Questao> Questoes { get; set; }
    public DbSet<Teste> Testes { get; set; }
    public DbSet<TesteMateriaQuantidade> QuantidadesPorMateria { get; set; }

    private readonly ITenantProvider? tenantProvider;

    public GeradorDeTestesDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null) : base(options)
    {
        this.tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
            modelBuilder.Entity<Alternativa>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
            modelBuilder.Entity<Disciplina>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
            modelBuilder.Entity<Materia>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
            modelBuilder.Entity<Questao>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
            modelBuilder.Entity<Teste>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
            modelBuilder.Entity<TesteMateriaQuantidade>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId.GetValueOrDefault()));
        }

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
