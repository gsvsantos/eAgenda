using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.ORM.ModuloCategoria;
using eAgenda.Infraestrutura.ORM.ModuloDespesa;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.Compartilhado;

public class EAgendaDbContext : DbContext
{
    public EAgendaDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Despesa> Despesas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MapeadorCategoriaORM());
        modelBuilder.ApplyConfiguration(new MapeadorDespesaORM());

        base.OnModelCreating(modelBuilder);
    }
}
