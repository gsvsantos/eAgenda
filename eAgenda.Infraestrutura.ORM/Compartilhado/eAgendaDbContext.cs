using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.Compartilhado;

public class EAgendaDbContext : DbContext
{
    public EAgendaDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Despesa> Despesas { get; set; }
    public DbSet<Compromisso> Compromissos { get; set; }
    public DbSet<Contato> Contatos { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<ItemTarefa> Itens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EAgendaDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
