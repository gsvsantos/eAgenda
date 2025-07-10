using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloCategoria;

public class RepositorioCategoriaORM : RepositorioBaseORM<Categoria>, IRepositorioCategoria
{
    public RepositorioCategoriaORM(EAgendaDbContext contexto) : base(contexto) { }

    public override Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {

        return registros.Where(c => c.Id.Equals(idRegistro)).Include(c => c.Despesas).FirstOrDefault();
    }

    public override List<Categoria> SelecionarRegistros()
    {
        return [.. registros.Include(c => c.Despesas)];
    }
}
