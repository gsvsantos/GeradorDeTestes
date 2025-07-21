using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
public class MapeadorTesteMateriaQuantidadeORM : IEntityTypeConfiguration<TesteMateriaQuantidade>
{
    public void Configure(EntityTypeBuilder<TesteMateriaQuantidade> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Materia)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.Property(x => x.QuantidadeQuestoes)
               .IsRequired();
    }
}
