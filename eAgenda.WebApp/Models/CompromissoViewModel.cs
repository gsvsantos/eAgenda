using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Models;

public abstract class FormularioCompromissoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Digite o Assunto.")]
    [DisplayName("Assunto")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O assunto do compromisso deve ter entre 2 e 100 caracteres.")]
    public string Assunto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione a data do compromisso.")]
    [DisplayName("Data do Compromisso")]
    [DataType(DataType.DateTime)]
    public DateTime DataOcorrencia { get; set; }

    [Required(ErrorMessage = "Insira o horário de início.")]
    [DisplayName("Hora de Início")]
    [DataType(DataType.DateTime)]
    public DateTime HoraInicio { get; set; }

    [Required(ErrorMessage = "Insira o horário de término.")]
    [DisplayName("Hora de Término")]
    [DataType(DataType.DateTime)]
    public DateTime HoraTermino { get; set; }

    [Required(ErrorMessage = "Selecione uma prioridade.")]
    [DisplayName("Prioridade")]
    public TipoCompromisso TipoCompromisso { get; set; }

    [DisplayName("Local")]
    [StringLength(100, ErrorMessage = "O local deve ter no máximo 100 caracteres.")]
    public string? Local { get; set; } = string.Empty;

    [DisplayName("Link")]
    [StringLength(200, ErrorMessage = "O link deve ter no máximo 200 caracteres.")]
    public string? Link { get; set; } = string.Empty;

    [DisplayName("Contato")]
    public Guid ContatoId { get; set; } = Guid.Empty;
    public List<SelectListItem> Contatos { get; set; } = [];
}

public class CadastrarCompromissoViewModel : FormularioCompromissoViewModel
{
    public CadastrarCompromissoViewModel() { }

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

public class EditarCompromissoViewModel : FormularioCompromissoViewModel
{
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
public class ExcluirCompromissoViewModel : FormularioCompromissoViewModel
{
    public ExcluirCompromissoViewModel() { }
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