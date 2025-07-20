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
            .IsRequired();

        builder.Property(t => t.Serie)
            .IsRequired();

        builder.HasMany(t => t.Materias)
            .WithMany(m => m.Testes);

        builder.Property(t => t.EhProvao)
            .IsRequired();

        builder.Property(t => t.QuantidadeQuestoes)
            .IsRequired();

        builder.HasMany(t => t.Questoes)
            .WithMany(d => d.Testes);

        builder.HasMany(t => t.QuantidadesPorMateria)
           .WithOne()
           .OnDelete(DeleteBehavior.Cascade);
    }
}
