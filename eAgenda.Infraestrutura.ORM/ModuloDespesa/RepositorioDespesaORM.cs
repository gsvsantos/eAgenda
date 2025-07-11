using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloDespesa;

public class RepositorioDespesaORM : RepositorioBaseORM<Despesa>, IRepositorioDespesa
{
    public RepositorioDespesaORM(EAgendaDbContext contexto) : base(contexto) { }

    public override Despesa? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Where(d => d.Id.Equals(idRegistro)).Include(d => d.Categorias).FirstOrDefault();
    }

    public override List<Despesa> SelecionarRegistros()
    {
        return registros.Include(d => d.Categorias).ToList();
    }

    public void AdicionarCategoria(Categoria categoria, Despesa despesa)
    {
        categoria.Despesas.Add(despesa);
    }

    public void RemoverCategoria(Categoria categoria, Despesa despesa)
    {
        categoria.Despesas.Remove(despesa);
    }
}
