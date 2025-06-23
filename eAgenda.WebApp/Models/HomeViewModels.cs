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

    //public HomeViewModel(int totalTarefas, int totalContatos, decimal totalDespesas, int totalCompromissos, int totalCategorias, List<string> ultimasTarefas, List<string> proximosCompromissos, List<string> ultimasDespesas)
    //{
    //    TotalTarefas = totalTarefas;
    //    TotalContatos = totalContatos;
    //    TotalDespesas = totalDespesas;
    //    TotalCompromissos = totalCompromissos;
    //    TotalCategorias = totalCategorias;
    //    UltimasTarefas = ultimasTarefas;
    //    ProximosCompromissos = proximosCompromissos;
    //    UltimasDespesas = ultimasDespesas;
    //}
}
