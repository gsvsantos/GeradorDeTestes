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
               .HasForeignKey(qmp => qmp.MateriaId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne(qmp => qmp.Teste)
               .WithMany(t => t.QuantidadesPorMateria)
               .HasForeignKey(qmp => qmp.TesteId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.Property(qmp => qmp.QuantidadeQuestoes)
               .IsRequired();
    }
}
