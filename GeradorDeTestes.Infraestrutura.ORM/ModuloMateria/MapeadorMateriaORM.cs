using GeradorDeTestes.Dominio.ModuloMateria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;

public class MapeadorMateriaORM : IEntityTypeConfiguration<Materia>
{
    public void Configure(EntityTypeBuilder<Materia> builder)
    {
        builder.Property(mate => mate.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(mate => mate.Nome)
            .IsRequired();

        builder.HasOne(mate => mate.Disciplina)
            .WithMany(disc => disc.Materias)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(mate => mate.Serie)
            .IsRequired();

        builder.HasMany(m => m.Questoes)
            .WithOne(q => q.Materia)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}