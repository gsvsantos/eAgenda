using eAgenda.Dominio.Compartilhado;
using System.ComponentModel.DataAnnotations;

namespace eAgenda.Dominio.ModuloTarefa;

public class Tarefa : EntidadeBase<Tarefa>
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public NivelPrioridade Prioridade { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataConclusao { get; set; }
    public StatusTarefa Status { get; set; }
    public double PercentualConcluido { get; set; }
    public List<ItemTarefa> Itens { get; set; } = [];

    private const double PercentualConclusao = 100;
    private const double PercentualPendencia = 0;

    public Tarefa(string titulo, string descricao, NivelPrioridade prioridade)
    {
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
        Status = StatusTarefa.Pendente;
        PercentualConcluido = 0;
    }

    public Tarefa(Guid id, string titulo, string descricao, NivelPrioridade prioridade, DateTime dataCriacao, DateTime? dataConclusao, StatusTarefa status) : this(titulo, descricao, prioridade)
    {
        Id = id;
        DataCriacao = dataCriacao;
        DataConclusao = dataConclusao;
        Status = status;
    }
    protected Tarefa() { }

    public void AdicionarItem(ItemTarefa item)
    {
        Itens.Add(item);
        AtualizarStatus();
    }

    public void RemoverItem(ItemTarefa item)
    {
        Itens.Remove(item);
        AtualizarStatus();
    }

    public void AtualizarStatus()
    {
        PercentualConcluido = CalcularPercentualConcluido();

        if (Itens.Count == 0)
            return;

        if (Status != StatusTarefa.Cancelada)
            Status = ObterStatusParaPercentual(PercentualConcluido);

        if (Status == StatusTarefa.Cancelada && Itens.Count != 0)
            Status = ObterStatusParaPercentual(PercentualConcluido);

        if (Status == StatusTarefa.Concluida)
            DataConclusao = DateTime.Now;

        if (Status == StatusTarefa.Pendente || Status == StatusTarefa.EmAndamento)
            DataConclusao = null;
    }

    public void Concluir()
    {
        foreach (ItemTarefa item in Itens)
            item.Concluir();

        DataConclusao = DateTime.Now;
        AtualizarStatus();
    }

    public void Reabrir()
    {
        foreach (ItemTarefa item in Itens)
            item.Reabrir();

        Status = StatusTarefa.Pendente;
        DataConclusao = null;
    }

    public void Cancelar()
    {
        foreach (ItemTarefa item in Itens)
            item.Cancelar();

        Status = StatusTarefa.Cancelada;
        DataConclusao = null;
    }

    public override void AtualizarRegistro(Tarefa registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Descricao = registroEditado.Descricao;
        Prioridade = registroEditado.Prioridade;
        DataCriacao = registroEditado.DataCriacao;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Tarefa tarefa)
            return false;

        return Id == tarefa.Id &&
               Titulo == tarefa.Titulo &&
               Descricao == tarefa.Descricao &&
               Prioridade == tarefa.Prioridade &&
               DataCriacao == tarefa.DataCriacao &&
               DataConclusao == tarefa.DataConclusao &&
               Status == tarefa.Status &&
               PercentualConcluido == tarefa.PercentualConcluido;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id, Titulo, Descricao,
            Prioridade, DataCriacao, DataConclusao,
            Status, PercentualConcluido);
    }

    private double CalcularPercentualConcluido()
    {
        if (Itens.Count == 0)
            return 0;

        int quantidadeConcluidos = Itens.Count(item => item.Status == StatusItemTarefa.Concluido);
        return (double)quantidadeConcluidos / Itens.Count * 100;
    }

    private StatusTarefa ObterStatusParaPercentual(double percentualStatus)
    {
        if (Itens.Any(i => i.Status == StatusItemTarefa.Cancelado))
            return StatusTarefa.Cancelada;

        if (percentualStatus >= PercentualConclusao)
            return StatusTarefa.Concluida;

        if (percentualStatus <= PercentualPendencia)
            return StatusTarefa.Pendente;

        return StatusTarefa.EmAndamento;
    }
}

public enum NivelPrioridade
{
    [Display(Name = "Baixa")]
    Baixa = 0,
    [Display(Name = "Média")]
    Media = 1,
    [Display(Name = "Alta")]
    Alta = 2
}

public enum StatusTarefa
{
    [Display(Name = "Pendente")]
    Pendente = 0,
    [Display(Name = "Em Andamento")]
    EmAndamento = 1,
    [Display(Name = "Concluída")]
    Concluida = 2,
    [Display(Name = "Cancelada")]
    Cancelada = 3
}