using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloTarefa;

public interface IRepositorioTarefa : IRepositorio<Tarefa>
{
    public void AtualizarStatusRegistros();
    public void ConcluirItensTarefa(Tarefa tarefa);
    public void ReabrirItensTarefa(Tarefa tarefa);
    public void CancelarItensTarefa(Tarefa tarefa);
    public List<Tarefa> SelecionarTarefasPorStatus(string? status);
    public List<Tarefa> SelecionarTarefasPorPrioridade(string? prioridade);
    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem);
}
