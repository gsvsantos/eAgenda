namespace eAgenda.WebApp.Models;

public class HomeViewModel
{
    public int TotalTarefas { get; set; }
    public int TotalContatos { get; set; }
    public decimal TotalDespesas { get; set; }
    public int TotalCompromissos { get; set; }
    public int TotalCategorias { get; set; }
    public List<string> UltimasTarefas { get; set; } = [];
    public List<string> ProximosCompromissos { get; set; } = [];
    public List<string> UltimasDespesas { get; set; } = [];
}
