using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloTarefa;

public class RepositorioTarefaORM : RepositorioBaseORM<Tarefa>, IRepositorioTarefa
{
    public RepositorioTarefaORM(EAgendaDbContext contexto) : base(contexto) { }

    public void AdicionarItem(ItemTarefa item)
    {
        throw new NotImplementedException();
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

    public void ConcluirItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public void ConcluirItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Concluir();
        }
    }

    public void ReabrirItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public void ReabrirItensTarefa(Tarefa tarefa)
    {
        foreach (ItemTarefa i in tarefa.Itens)
        {
            i.Reabrir();
        }
    }

    public void RemoverItem(ItemTarefa item)
    {
        throw new NotImplementedException();
    }

    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem)
    {
        return tarefa.Itens.FirstOrDefault(i => i.Id == idItem);
    }

    public List<Tarefa> SelecionarTarefasPorPrioridade(string? prioridade)
    {
        throw new NotImplementedException();
    }

    public List<Tarefa> SelecionarTarefasPorStatus(string? status)
    {
        throw new NotImplementedException();
    }

    public override Tarefa? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros
            .Include(t => t.Itens)
            .FirstOrDefault(t => t.Id.Equals(idRegistro));
    }
    public override List<Tarefa> SelecionarRegistros()
    {
        return registros
            .Include(t => t.Itens)
            .ToList();
    }
}
