using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloTarefa;

public interface IRepositorioTarefa : IRepositorio<Tarefa>
{
    public void AtualizarStatusRegistros();
    public void AdicionarItem(ItemTarefa item);
    public void RemoverItem(ItemTarefa item);
    public void ConcluirItem(ItemTarefa item);
    public void ReabrirItem(ItemTarefa item);
    public void ConcluirItensTarefa(Tarefa tarefa);
    public void ReabrirItensTarefa(Tarefa tarefa);
    public void CancelarItensTarefa(Tarefa tarefa);
    public List<Tarefa> SelecionarTarefasPendentes();
    public List<Tarefa> SelecionarTarefasEmAndamento();
    public List<Tarefa> SelecionarTarefasConcluidas();
    public List<Tarefa> SelecionarTarefasCanceladas();
    public List<Tarefa> SelecionarTarefasPrioridadeBaixa();
    public List<Tarefa> SelecionarTarefasPrioridadeMedia();
    public List<Tarefa> SelecionarTarefasPrioridadeAlta();
    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem);
}
