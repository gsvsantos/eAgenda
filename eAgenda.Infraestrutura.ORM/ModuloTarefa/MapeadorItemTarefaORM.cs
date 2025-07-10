using eAgenda.Dominio.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloTarefa;

public class MapeadorItemTarefaORM : IEntityTypeConfiguration<ItemTarefa>
{
    public void Configure(EntityTypeBuilder<ItemTarefa> builder)
    {
        builder.ToTable("TBItemTarefa");

        builder.Property(i => i.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(i => i.Titulo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired();

        builder.HasOne(i => i.Tarefa)
            .WithMany(t => t.Itens)
            .IsRequired();
    }
}
