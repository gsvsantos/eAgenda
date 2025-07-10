using eAgenda.Dominio.ModuloContato;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloConta;

public class MapeadorContatoORM : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.ToTable("TBContato");

        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Cargo)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(c => c.Empresa)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.HasMany(c => c.Compromissos)
            .WithOne(com => com.Contato)
            .IsRequired(false);
    }
}
