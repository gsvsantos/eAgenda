using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloTarefa;

public class RepositorioTarefaORM : RepositorioBaseORM<Tarefa>, IRepositorioTarefa
{
    public RepositorioTarefaORM(EAgendaDbContext contexto) : base(contexto) { }

    public override Tarefa? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros
            .Include(t => t.Itens)
            .FirstOrDefault(t => t.Id.Equals(idRegistro));
    }

    public override List<Tarefa> SelecionarRegistros()
    {
        return [.. registros.Include(t => t.Itens)];
    }

    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem)
    {
        return tarefa.Itens.FirstOrDefault(i => i.Id == idItem);
    }

    public List<Tarefa> SelecionarTarefasPorPrioridade(string? prioridade)
    {
        NivelPrioridade? prioridadeAtual = prioridade switch
        {
            "Baixa" => NivelPrioridade.Baixa,
            "Media" => NivelPrioridade.Media,
            "Alta" => NivelPrioridade.Alta,
            _ => null
        };

        return [.. registros
            .Where(t => t.Prioridade.Equals(prioridadeAtual))
            .Include(t => t.Itens)];
    }

    public List<Tarefa> SelecionarTarefasPorStatus(string? status)
    {
        StatusTarefa? statusAtual = status switch
        {
            "Pendente" => StatusTarefa.Pendente,
            "EmAndamento" => StatusTarefa.EmAndamento,
            "Concluida" => StatusTarefa.Concluida,
            "Cancelada" => StatusTarefa.Cancelada,
            _ => null
        };

        return [.. registros
            .Where(t => t.Status.Equals(statusAtual))
            .Include(t => t.Itens)];
    }

    public void AtualizarStatusRegistros()
    {
        foreach (Tarefa tarefa in SelecionarRegistros())
        {
            tarefa.AtualizarStatus();
        }
    }

    public void CancelarItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Cancelar();
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
}
