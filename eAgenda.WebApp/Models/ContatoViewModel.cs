using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApp.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eAgenda.WebApp.Models;

public abstract class FormularioContatoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Insira um Nome.")]
    [DisplayName("Nome")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome do contato deve ter entre 2 e 255 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insira um Telefone.")]
    [DisplayName("Telefone")]
    [StringLength(30, ErrorMessage = "O Telefone não pode conter mais que 30 caracteres.")]
    [RegularExpression("^\\(?\\d{2}\\)?\\s?(9\\d{4}|\\d{4})-?\\d{4}$", ErrorMessage = "Número inválido.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insira um Email.")]
    [DisplayName("Email")]
    [EmailAddress(ErrorMessage = "Insira um endereço de email válido.")]
    [StringLength(254, MinimumLength = 6, ErrorMessage = "O email deve ter entre 6 e 254 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [DisplayName("Cargo")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome do item deve ter entre 2 e 150 caracteres.")]
    public string? Cargo { get; set; }

    [DisplayName("Empresa")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O nome do item deve ter entre 2 e 200 caracteres.")]
    public string? Empresa { get; set; }
    public List<CompromissoContatoViewModel> Compromissos { get; set; } = [];
}

public class CadastrarContatoViewModel : FormularioContatoViewModel
{
    public CadastrarContatoViewModel() { }
    public CadastrarContatoViewModel(Guid id, string nome, string telefone, string email, string cargo, string empresa, List<Compromisso> compromissos) : this()
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Email = email;
        Cargo = cargo;
        Empresa = empresa;
        foreach (Compromisso c in compromissos)
        {
            Compromissos.Add(new CompromissoContatoViewModel(
                c.Assunto,
                c.DataOcorrencia,
                c.HoraInicio,
                c.HoraTermino,
                c.TipoCompromisso,
                c.Local,
                c.Link,
                c.Contato!.Nome));
        }
    }
}
public class VisualizarContatosViewModel
{
    public List<DetalhesContatoViewModel> Registros { get; set; } = [];

    public VisualizarContatosViewModel(List<Contato> contatos)
    {
        foreach (Contato contato in contatos)
        {
            Registros.Add(contato.ParaDetalhesVM());
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
    public ExcluirContatoViewModel(Guid id, string nome) : this()
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
                compromisso.Link,
                compromisso.Contato!.Nome
            ));
        }
    }
}
public class CompromissoContatoViewModel
{
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraTermino { get; set; }

    public TipoCompromisso TipoCompromisso { get; set; }

    public string Local { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;
    public string? NomeContato { get; set; } = string.Empty;

    public CompromissoContatoViewModel(string assunto, DateTime dataOcorrencia, TimeSpan horaInicio, TimeSpan horaTermino, TipoCompromisso tipoCompromisso, string local, string link, string? nomeContato)
    {
        Assunto = assunto;
        DataOcorrencia = dataOcorrencia;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        TipoCompromisso = tipoCompromisso;
        Local = local;
        Link = link;
        NomeContato = nomeContato;
    }
}