using GeradorDeTestes.Dominio.ModuloDisciplina;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;

public class MapeadorDisciplinaORM : IEntityTypeConfiguration<Disciplina>
{
    public void Configure(EntityTypeBuilder<Disciplina> builder)
    {
        builder.Property(disc => disc.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(disc => disc.Nome)
            .IsRequired();

        builder.HasMany(disc => disc.Materias)
            .WithOne(mate => mate.Disciplina)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(disc => disc.Testes)
            .WithOne(test => test.Disciplina)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
