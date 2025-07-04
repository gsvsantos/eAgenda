using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCategoria;

namespace eAgenda.Dominio.ModuloDespesa;

public interface IRepositorioDespesa : IRepositorio<Despesa>
{
    public void AdicionarCategoria(Categoria categoria, Despesa despesa);
    public void RemoverCategoria(Categoria categoria, Despesa despesa);
}