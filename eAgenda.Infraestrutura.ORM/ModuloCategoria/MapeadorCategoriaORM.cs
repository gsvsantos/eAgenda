using eAgenda.Dominio.ModuloCategoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloCategoria;

public class MapeadorCategoriaORM : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("TBCategoria");

        builder.Property(c => c.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(c => c.Titulo)
               .HasMaxLength(100)
               .IsRequired();

        builder.HasMany(c => c.Despesas)
               .WithMany(d => d.Categorias);
    }
}
