using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Models;

public abstract class FormularioTarefaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Escreva um título.")]
    [DisplayName("Título")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O título da tarefa deve ter entre 2 e 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [DisplayName("Descrição")]
    public string? Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione uma prioridade.")]
    [DisplayName("Prioridade")]
    public NivelPrioridade? Prioridade { get; set; }

    [Required(ErrorMessage = "Selecione uma data.")]
    [DisplayName("Data de Criação ")]
    [DataType(DataType.DateTime)]
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}

public class CadastrarTarefaViewModel : FormularioTarefaViewModel
{
    public CadastrarTarefaViewModel() { }
    public CadastrarTarefaViewModel(string titulo, NivelPrioridade prioridade)
    {
        Titulo = titulo;
        Prioridade = prioridade;
    }
}

public class VisualizarTarefasViewModel
{
    public Dictionary<NivelPrioridade, List<DetalhesTarefaViewModel>> TarefasPorPrioridade { get; } = [];

    public VisualizarTarefasViewModel(List<Tarefa> tarefas)
    {
        foreach (NivelPrioridade prioridade in Enum.GetValues(typeof(NivelPrioridade)))
        {
            TarefasPorPrioridade[prioridade] = [.. tarefas
                .Where(t => t.Prioridade == prioridade)
                .Select(t => t.ParaDetalhesVM())];
        }
    }
}

public class EditarTarefaViewModel : FormularioTarefaViewModel
{
    public EditarTarefaViewModel() { }
    public EditarTarefaViewModel(Guid id, string titulo, NivelPrioridade prioridade, DateTime dataCriacao, string? descricao)
    {
        Id = id;
        Titulo = titulo;
        Prioridade = prioridade;
        DataCriacao = dataCriacao;
        Descricao = descricao;
    }
}

public class ExcluirTarefaViewModel : FormularioTarefaViewModel
{
    public ExcluirTarefaViewModel() { }
    public ExcluirTarefaViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}

public class DetalhesTarefaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public NivelPrioridade Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataConclusao { get; set; }
    public StatusTarefa Status { get; set; }
    public double PercentualConcluido { get; set; }
    public List<ItemTarefaViewModel> Itens { get; set; } = [];

    public DetalhesTarefaViewModel(Guid id, string titulo, string descricao, NivelPrioridade prioridade, DateTime dataCriacao, DateTime dataConclusao, StatusTarefa status, double percentualConcluido, List<ItemTarefa> itens)
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
        DataCriacao = dataCriacao;
        DataConclusao = dataConclusao;
        Status = status;
        PercentualConcluido = percentualConcluido;

        foreach (ItemTarefa i in itens)
        {
            Itens.Add(new ItemTarefaViewModel(
                 i.Id,
                 i.Titulo,
                 i.Status));
        }
    }
}

public class GerenciarItensViewModel
{
    public DetalhesTarefaViewModel Tarefa { get; set; } = null!;
    public List<SelectListItem> Itens { get; set; } = [];

    [Required(ErrorMessage = "Escreva um título.")]
    [DisplayName("Título")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O título da tarefa deve ter entre 2 e 100 caracteres.")]
    public string TituloItem { get; set; } = string.Empty;

    public GerenciarItensViewModel() { }
    public GerenciarItensViewModel(Tarefa tarefa, List<ItemTarefa> itens) : this()
    {
        Tarefa = tarefa.ParaDetalhesVM();

        foreach (ItemTarefa item in itens)
        {
            Itens.Add(new SelectListItem
            {
                Text = item.Titulo,
                Value = item.Id.ToString()
            });
        }
    }
}

public class ItemTarefaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public StatusItemTarefa Status { get; set; }
    public ItemTarefaViewModel() { }
    public ItemTarefaViewModel(Guid id, string titulo, StatusItemTarefa status) : this()
    {
        Id = id;
        Titulo = titulo;
        Status = status;
    }
}

public class AdicionarItemViewModel
{
    public string TituloItem { get; set; } = string.Empty;
}
