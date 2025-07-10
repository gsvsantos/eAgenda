using eAgenda.Dominio.ModuloDespesa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloDespesa;

public class MapeadorDespesaORM : IEntityTypeConfiguration<Despesa>
{
    public void Configure(EntityTypeBuilder<Despesa> builder)
    {
        builder.ToTable("TBDespesa");

        builder.Property(d => d.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(d => d.Titulo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(d => d.Descricao)
               .HasMaxLength(1000)
               .IsRequired();

        builder.Property(d => d.DataOcorrencia)
               .IsRequired();

        builder.Property(d => d.Valor)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(d => d.FormaPagamento)
               .IsRequired();

        builder.HasMany(d => d.Categorias)
               .WithMany(c => c.Despesas)
               .UsingEntity(j => j.ToTable("TBDespesa_TBCategoria"));
    }
}
