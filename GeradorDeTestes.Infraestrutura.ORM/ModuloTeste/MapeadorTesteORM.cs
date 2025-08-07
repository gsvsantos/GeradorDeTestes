using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;

public class MapeadorTesteORM : IEntityTypeConfiguration<Teste>
{
    public void Configure(EntityTypeBuilder<Teste> builder)
    {
        builder.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.Titulo)
            .IsRequired();

        builder.HasOne(t => t.Disciplina)
            .WithMany(d => d.Testes)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(t => t.Serie)
            .IsRequired();

        builder.HasMany(t => t.Materias)
            .WithMany(m => m.Testes)
            .UsingEntity<Dictionary<string, object>>(
            "MateriaTeste", j => j
            .HasOne<Materia>()
            .WithMany().HasForeignKey("MateriasId")
            .OnDelete(DeleteBehavior.Cascade), j => j
            .HasOne<Teste>()
            .WithMany().HasForeignKey("TestesId")
            .OnDelete(DeleteBehavior.Cascade));

        builder.Property(t => t.EhProvao)
            .IsRequired();

        builder.Property(t => t.QuantidadeQuestoes)
            .IsRequired();

        builder.Property(t => t.Finalizado)
            .IsRequired();

        builder.Property(t => t.DataCriacao)
            .IsRequired();

        builder.HasMany(t => t.Questoes)
            .WithMany(d => d.Testes)
            .UsingEntity<Dictionary<string, object>>(
            "QuestaoTeste",
            j => j
            .HasOne<Questao>()
            .WithMany()
            .HasForeignKey("QuestoesId")
            .OnDelete(DeleteBehavior.Cascade),
            j => j
            .HasOne<Teste>()
            .WithMany()
            .HasForeignKey("TestesId")
            .OnDelete(DeleteBehavior.Cascade));

        builder.HasMany(t => t.QuantidadesPorMateria)
            .WithOne(qmp => qmp.Teste)
            .HasForeignKey(qmp => qmp.TesteId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
