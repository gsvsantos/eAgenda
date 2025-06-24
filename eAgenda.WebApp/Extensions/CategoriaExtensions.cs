using eAgenda.Dominio.ModuloCategoria;
using eAgenda.WebApp.Models;

namespace eAgenda.WebApp.Extensions;

public static class CategoriaExtensions
{
    public static Categoria ParaEntidade(this FormularioCategoriaViewModel formularioVM)
    {
        return new Categoria(formularioVM.Titulo);
    }

    public static DespesasCategoriaViewModel ParaDetalhesVM(this Categoria categoria)
    {
        return new DespesasCategoriaViewModel(
                categoria.Id,
                categoria.Titulo,
                categoria.Despesas
        );
    }
}
