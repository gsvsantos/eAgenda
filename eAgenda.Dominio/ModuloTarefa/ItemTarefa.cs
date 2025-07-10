using System.ComponentModel.DataAnnotations;

namespace eAgenda.Dominio.ModuloTarefa;

public class ItemTarefa
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public StatusItemTarefa Status { get; set; }
    public Tarefa Tarefa { get; set; }

    public ItemTarefa(string titulo, Tarefa tarefa)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Tarefa = tarefa;
        Status = StatusItemTarefa.EmAndamento;
    }
    protected ItemTarefa() { }

    public ItemTarefa(Guid id, string titulo, StatusItemTarefa status, Tarefa tarefa) : this(titulo, tarefa)
    {
        Id = id;
        Status = status;
    }

    public void MarcarEmAndamento()
    {
        Status = StatusItemTarefa.EmAndamento;
    }

    public void Concluir()
    {
        Status = StatusItemTarefa.Concluido;
    }

    public void Reabrir()
    {
        if (Status == StatusItemTarefa.Concluido || Status == StatusItemTarefa.Cancelado)
            Status = StatusItemTarefa.EmAndamento;
    }

    public void Cancelar()
    {
        Status = StatusItemTarefa.Cancelado;
    }
}

public enum StatusItemTarefa
{
    [Display(Name = "Em Andamento")]
    EmAndamento = 0,
    [Display(Name = "Concluído")]
    Concluido = 1,
    [Display(Name = "Cancelado")]
    Cancelado = 2,
}