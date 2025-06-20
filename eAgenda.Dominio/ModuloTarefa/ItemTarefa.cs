using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace eAgenda.Dominio.ModuloTarefa;

public class ItemTarefa
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public StatusItemTarefa Status { get; set; }

    [ExcludeFromCodeCoverage]
    public ItemTarefa() { }
    public ItemTarefa(string titulo) : this()
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Status = StatusItemTarefa.EmAndamento;
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