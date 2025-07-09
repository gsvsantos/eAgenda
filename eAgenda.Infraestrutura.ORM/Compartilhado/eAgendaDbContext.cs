using eAgenda.Dominio.ModuloCategoria;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.Compartilhado;

public class eAgendaDbContext : DbContext
{
    public eAgendaDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Categoria> Categorias { get; set; }
}
