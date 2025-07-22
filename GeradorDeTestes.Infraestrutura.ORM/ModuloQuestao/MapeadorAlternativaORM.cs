using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
public class MapeadorAlternativaORM : IEntityTypeConfiguration<Alternativa>
{
    public void Configure(EntityTypeBuilder<Alternativa> builder)
    {
        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(a => a.Texto)
            .IsRequired();

        builder.HasOne(a => a.Questao)
            .WithMany(q => q.Alternativas)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(a => a.EstaCorreta)
            .IsRequired();
    }
}
