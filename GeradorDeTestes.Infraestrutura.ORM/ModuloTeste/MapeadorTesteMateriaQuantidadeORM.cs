using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
public class MapeadorTesteMateriaQuantidadeORM : IEntityTypeConfiguration<TesteMateriaQuantidade>
{
    public void Configure(EntityTypeBuilder<TesteMateriaQuantidade> builder)
    {
        builder.Property(qmp => qmp.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasOne(qmp => qmp.Materia)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne(qpm => qpm.Teste)
               .WithMany(t => t.QuantidadesPorMateria)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.Property(qmp => qmp.QuantidadeQuestoes)
               .IsRequired();
    }
}
