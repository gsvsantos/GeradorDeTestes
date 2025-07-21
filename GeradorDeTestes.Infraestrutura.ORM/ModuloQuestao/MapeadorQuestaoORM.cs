using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
public class MapeadorQuestaoORM : IEntityTypeConfiguration<Questao>
{
    public void Configure(EntityTypeBuilder<Questao> builder)
    {
        builder.Property(q => q.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(q => q.Enunciado)
            .IsRequired();

        builder.HasOne(q => q.Materia)
            .WithMany(m => m.Questoes)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(q => q.Finalizado)
            .IsRequired();

        builder.Property(q => q.DataCriacao)
            .IsRequired();

        builder.HasMany(q => q.Alternativas)
            .WithOne(a => a.Questao)
            .IsRequired();

        builder.HasMany(q => q.Testes)
            .WithMany(t => t.Questoes);
    }
}
