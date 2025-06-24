using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;

namespace eAgenda.Infraestrutura.Arquivos.ModuloCategoria;

public class RepositorioCategoriaEmArquivo : RepositorioBaseEmArquivo<Categoria>, IRepositorioCategoria
{
#pragma warning disable IDE0290 // Use primary constructor
    public RepositorioCategoriaEmArquivo(ContextoDados contexto) : base(contexto) { }
#pragma warning restore IDE0290 // Use primary constructor

    protected override List<Categoria> ObterRegistros()
    {
        return contexto.Categorias;
    }
}
