using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;

namespace eAgenda.Infraestrutura.Arquivos.ModuloDespesa;

public class RepositorioDespesaEmArquivo : RepositorioBaseEmArquivo<Despesa>, IRepositorioDespesa
{
    public RepositorioDespesaEmArquivo(ContextoDados contexto) : base(contexto) { }

    protected override List<Despesa> ObterRegistros()
    {
        return contexto.Despesas;
    }

    public Despesa SelecionarPorId(Guid idRegistro)
    {
        foreach (var item in registros)
        {
            if (item.Id == idRegistro)
                return item;
        }

        return null;
    }

}
