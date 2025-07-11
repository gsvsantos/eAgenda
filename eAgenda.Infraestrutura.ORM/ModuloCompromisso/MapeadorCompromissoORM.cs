using eAgenda.Dominio.ModuloCompromisso;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infraestrutura.ORM.ModuloCompromisso;

public class MapeadorCompromissoORM : IEntityTypeConfiguration<Compromisso>
{
    public void Configure(EntityTypeBuilder<Compromisso> builder)
    {
        builder.ToTable("TBCompromisso");

        builder.Property(c => c.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(c => c.Assunto)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(c => c.DataOcorrencia)
               .IsRequired();

        builder.Property(c => c.HoraInicio)
               .IsRequired();

        builder.Property(c => c.HoraTermino)
               .IsRequired();

        builder.Property(c => c.TipoCompromisso)
               .IsRequired();

        builder.Property(c => c.Local)
               .HasMaxLength(255)
               .IsRequired(false);

        builder.Property(c => c.Link)
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.HasOne(c => c.Contato)
               .WithMany(cont => cont.Compromissos)
               .IsRequired(false);
    }
}
