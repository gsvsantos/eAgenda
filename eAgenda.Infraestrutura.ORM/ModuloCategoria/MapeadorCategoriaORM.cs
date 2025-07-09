using eAgenda.Dominio.ModuloCategoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloCategoria;

public class MapeadorCategoriaORM : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Titulo)
            .IsRequired();

        builder.Property(c => c.Despesas);
    }
}
