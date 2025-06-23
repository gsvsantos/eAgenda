using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Models;

public abstract class FormularioContatoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Empresa { get; set; }
}
public class CadastrarContatoViewModel : FormularioContatoViewModel
{
    public CadastrarContatoViewModel()
    {

    }
    public CadastrarContatoViewModel(Guid id, string nome, string telefone, string email, string cargo, string empresa) : this()
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Cargo = cargo;
        Empresa = empresa;
    }
}
public class VisualizarContatosViewModel
{

    public List<DetalhesContatoViewModel> Registros { get; set; } = [];

    public VisualizarContatosViewModel(List<Contato> contatos)
    {
        foreach (Contato contato in contatos)
        {
            Registros.Add(new DetalhesContatoViewModel(
                contato.Id,
                contato.Nome,
                contato.Email,
                contato.Telefone,
                contato.Cargo,
                contato.Empresa,
                contato.Compromissos));
        }
    }
}
public class EditarContatoViewModel : FormularioContatoViewModel
{
    public EditarContatoViewModel()
    {

    }
    public EditarContatoViewModel(Guid id, string nome, string telefone, string email, string cargo, string empresa) : this()
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Cargo = cargo;
        Empresa = empresa;
    }
}

public class ExcluirContatoViewModel : FormularioContatoViewModel
{
    public ExcluirContatoViewModel()
    {

    }
    public ExcluirContatoViewModel(Guid id, string nome)    
    {
        Id = id;
        Nome = nome;
    }
}
public class DetalhesContatoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Cargo { get; set; }
    public string Empresa { get; set; }
    public List<CompromissoContatoViewModel> Compromissos { get; set; } = [];

    public DetalhesContatoViewModel(Guid id, string nome, string email, string telefone, string cargo, string empresa, List<Compromisso> compromissos)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Telefone = telefone;
        Cargo = cargo;
        Empresa = empresa;

        foreach (Compromisso compromisso in compromissos)
        {
            Compromissos.Add(new CompromissoContatoViewModel
            (
                compromisso.Assunto,
                compromisso.DataOcorrencia,
                compromisso.HoraInicio,
                compromisso.HoraTermino,
                compromisso.TipoCompromisso,
                compromisso.Local,
                compromisso.Link
            ));
        }
    }
}
public class CompromissoContatoViewModel
{
    public CompromissoContatoViewModel(string assunto, DateTime dataOcorrencia, DateTime horaInicio, DateTime horaTermino, TipoCompromisso tipoCompromisso, string local, string link)
    {
        Assunto = assunto;
        DataOcorrencia = dataOcorrencia;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        TipoCompromisso = tipoCompromisso;
        Local = local;
        Link = link;
    }

    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }

    public DateTime HoraInicio { get; set; }

    public DateTime HoraTermino { get; set; }

    public TipoCompromisso TipoCompromisso { get; set; }

    public string Local { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

}