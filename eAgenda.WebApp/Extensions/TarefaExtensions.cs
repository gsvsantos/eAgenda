using eAgenda.Dominio.ModuloTarefa;
using eAgenda.WebApp.Models;

namespace eAgenda.WebApp.Extensions;

public static class TarefaExtensions
{
    public static Tarefa ParaEntidade(this FormularioTarefaViewModel formularioVM)
    {
        return new(
            formularioVM.Titulo,
            formularioVM.Descricao ?? string.Empty,
            formularioVM.Prioridade ?? NivelPrioridade.Baixa);
    }
    public static DetalhesTarefaViewModel ParaDetalhesVM(this Tarefa tarefa)
    {
        return new(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Prioridade,
            tarefa.DataCriacao,
            tarefa.DataConclusao,
            tarefa.Status,
            tarefa.PercentualConcluido,
            tarefa.Itens);
    }
}
