using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace eAgenda.WebApp.Models;

public class CadastrarCompromissoViewModel
{
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraTermino { get; set; }
    public TipoCompromisso TipoCompromisso { get; set; }
    public string? Local { get; set; } = string.Empty;
    public string? Link { get; set; } = string.Empty;
    public Guid ContatoId { get; set; } = Guid.Empty;

    public List<SelectListItem> Contatos { get; set; } = [];

    public CadastrarCompromissoViewModel()
    {

    }

    public CadastrarCompromissoViewModel(List<Contato> contatos)
    {
        foreach (var contato in contatos)
        {
            Contatos.Add(new SelectListItem
            {
                Value = contato.Id.ToString(),
                Text = contato.Nome
            });
        }
    }
}

public class EditarCompromissoViewModel : CadastrarCompromissoViewModel
{
    public Guid Id { get; set; }

    public EditarCompromissoViewModel() { }

    public EditarCompromissoViewModel(Compromisso compromisso, List<Contato> contatos)
    {
        Id = compromisso.Id;
        Assunto = compromisso.Assunto;
        DataOcorrencia = compromisso.DataOcorrencia;
        HoraInicio = compromisso.HoraInicio;
        HoraTermino = compromisso.HoraTermino;
        TipoCompromisso = compromisso.TipoCompromisso;
        Local = compromisso.Local;
        Link = compromisso.Link;
        ContatoId = compromisso.Contato != null ? compromisso.Contato.Id : Guid.Empty;
        foreach (var contato in contatos)
        {
            Contatos.Add(new SelectListItem
            {
                Value = contato.Id.ToString(),
                Text = contato.Nome
            });
        }
    }
}
public class ExcluirCompromissoViewModel
{
    public Guid Id { get; set; }
    public string Assunto { get; set; }

    public ExcluirCompromissoViewModel(Guid id, string assunto)
    {
        Id = id;
        Assunto = assunto;
    }
}
public class VisualizarCompromissosViewModel
{
    public List<DetalhesCompromissoViewModel> Registros { get; set; } = [];
    public VisualizarCompromissosViewModel(List<Compromisso> compromissos)
    {
        foreach (Compromisso compromisso in compromissos)
        {
            Registros.Add(new DetalhesCompromissoViewModel(
                compromisso.Id,
                compromisso.Assunto,
                compromisso.DataOcorrencia,
                compromisso.HoraInicio,
                compromisso.HoraTermino,
                compromisso.TipoCompromisso,
                compromisso.Local,
                compromisso.Link,
                compromisso.Contato));
        }
    }
}
public class DetalhesCompromissoViewModel
{
    public Guid Id { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }

    public DateTime HoraInicio { get; set; }

    public DateTime HoraTermino { get; set; }

    public TipoCompromisso TipoCompromisso { get; set; }

    public string Local { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public Contato? Contato { get; set; } = null!;
    public DetalhesCompromissoViewModel(Guid id, string assunto, DateTime dataOcorrencia, DateTime horaInicio, DateTime horaTermino, TipoCompromisso tipoCompromisso, string local, string link, Contato? contato)
    {
        Id = id;
        Assunto = assunto;
        DataOcorrencia = dataOcorrencia;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        TipoCompromisso = tipoCompromisso;
        Local = local;
        Link = link;
        Contato = contato;
    }
}





