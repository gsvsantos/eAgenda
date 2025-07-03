using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;

namespace eAgenda.Infraestrutura.Arquivos.ModuloTarefa;

public class RepositorioTarefaEmArquivos : RepositorioBaseEmArquivo<Tarefa>, IRepositorioTarefa
{
    public RepositorioTarefaEmArquivos(ContextoDados contexto) : base(contexto) { }

    public void AtualizarStatusRegistros()
    {
        foreach (Tarefa t in registros)
        {
            t.AtualizarStatus();
        }
    }

    public void ConcluirItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Concluir();
        }
    }

    public void ReabrirItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Reabrir();
        }
    }

    public void CancelarItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Cancelar();
        }
    }

    public ItemTarefa SelecionarItem(Tarefa tarefa, Guid idItem)
    {
        ItemTarefa item = null!;

        foreach (ItemTarefa i in tarefa.Itens)
        {
            if (i.Id == idItem)
            {
                item = i;
                break;
            }
        }

        return item;
    }

    public List<Tarefa> SelecionarTarefasPendentes()
    {
        return [.. registros.Where(c => c.Status == StatusTarefa.Pendente)];
    }

    public List<Tarefa> SelecionarTarefasEmAndamento()
    {
        return [.. registros.Where(c => c.Status == StatusTarefa.EmAndamento)];
    }

    public List<Tarefa> SelecionarTarefasConcluidas()
    {
        return [.. registros.Where(c => c.Status == StatusTarefa.Concluida)];
    }

    public List<Tarefa> SelecionarTarefasCanceladas()
    {
        return [.. registros.Where(c => c.Status == StatusTarefa.Cancelada)];
    }

    public List<Tarefa> SelecionarTarefasPrioridadeBaixa()
    {
        return [.. registros.Where(c => c.Prioridade == NivelPrioridade.Baixa)];
    }

    public List<Tarefa> SelecionarTarefasPrioridadeMedia()
    {
        return [.. registros.Where(c => c.Prioridade == NivelPrioridade.Media)];
    }

    public List<Tarefa> SelecionarTarefasPrioridadeAlta()
    {
        return [.. registros.Where(c => c.Prioridade == NivelPrioridade.Alta)];
    }

    protected override List<Tarefa> ObterRegistros()
    {
        return contexto.Tarefas;
    }

    public void AdicionarItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public void RemoverItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public void ConcluirItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public void ReabrirItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }
}
