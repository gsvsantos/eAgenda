using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloContato;

namespace eAgenda.Dominio.ModuloCompromisso;

public class Compromisso : EntidadeBase<Compromisso>
{
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }

    public DateTime HoraInicio { get; set; }

    public DateTime HoraTermino { get; set; }

    public TipoCompromisso TipoCompromisso { get; set; }

    public string Local { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public Contato? Contato { get; set; } = null!;

    [ExcludeFromCodeCoverage]
    public Compromisso() { }
    public Compromisso(string assunto, DateTime dataOcorrencia, DateTime horaInicio, DateTime horaTermino,
        TipoCompromisso tipoCompromisso, string local, string link, Contato? contato) : this()
    {
        Assunto = assunto;
        DataOcorrencia = dataOcorrencia;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        TipoCompromisso = tipoCompromisso;
        Local = local;
        Link = link;
        Contato = contato;
    }
    public void Iniciar()
    {
        HoraInicio = DateTime.Now;
    }
    public void Terminar()
    {
        HoraTermino = DateTime.Now;
    }
    public override void AtualizarRegistro(Compromisso registroEditado)
    {
        Assunto = registroEditado.Assunto;
        TipoCompromisso = registroEditado.TipoCompromisso;
        Local = registroEditado.Local;
        Link = registroEditado.Link;
        Contato = registroEditado.Contato;
    }
    public List<string> Validar()
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(Assunto) || Assunto.Length < 2 || Assunto.Length > 100)
            erros.Add("Assunto deve ter entre 2 e 100 caracteres");
        if (DataOcorrencia < DateTime.Now)
            erros.Add("Data de ocorrência não pode ser no passado");
        if (HoraInicio >= HoraTermino)
            erros.Add("Hora de início deve ser anterior à hora de término");
        if (TipoCompromisso == TipoCompromisso.Presencial && string.IsNullOrWhiteSpace(Local))
            erros.Add("Local é obrigatório para compromissos presenciais");
        if (TipoCompromisso == TipoCompromisso.Remoto && string.IsNullOrWhiteSpace(Link))
            erros.Add("Link é obrigatório para compromissos remotos");
        return erros;
    }
}
public enum TipoCompromisso
{
    [Display(Name = "Remoto")]
    Remoto = 0,
    [Display(Name = "Presencial")]
    Presencial = 1
}