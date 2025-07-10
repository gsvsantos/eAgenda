using eAgenda.Dominio.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloTarefa;

public class MapeadorTarefaORM : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("TBTarefa");

        builder.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.Titulo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Descricao)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(t => t.Prioridade)
            .IsRequired();

        builder.Property(t => t.DataCriacao)
            .IsRequired();

        builder.Property(t => t.DataConclusao)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasMany(t => t.Itens)
            .WithOne(i => i.Tarefa);
    }
}
