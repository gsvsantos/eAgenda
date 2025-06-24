using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;

namespace eAgenda.WebApp.Models;

public abstract class FormularioContatoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Insira um Nome.")]
    [DisplayName("Nome")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do contato deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insira um Telefone.")]
    [DisplayName("Telefone")]
    [RegularExpression("^\\(?\\d{2}\\)?\\s?(9\\d{4}|\\d{4})-?\\d{4}$", ErrorMessage = "Número inválido.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insira um Email.")]
    [DisplayName("Email")]
    [EmailAddress(ErrorMessage = "Insira um endereço de email válido.")]
    public string Email { get; set; } = string.Empty;

    [DisplayName("Cargo")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do item deve ter entre 2 e 100 caracteres.")]
    public string? Cargo { get; set; }

    [DisplayName("Empresa")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do item deve ter entre 2 e 100 caracteres.")]
    public string? Empresa { get; set; }
}
public class CadastrarContatoViewModel : FormularioContatoViewModel
{
    public CadastrarContatoViewModel() { }
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
    public EditarContatoViewModel() { }
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
    public ExcluirContatoViewModel() { }
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
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }

    public DateTime HoraInicio { get; set; }

    public DateTime HoraTermino { get; set; }

    public TipoCompromisso TipoCompromisso { get; set; }

    public string Local { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

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
}