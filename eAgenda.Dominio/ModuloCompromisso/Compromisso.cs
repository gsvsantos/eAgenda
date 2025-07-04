using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloContato;

namespace eAgenda.Dominio.ModuloCompromisso;

public class Compromisso : EntidadeBase<Compromisso>
{
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraTermino { get; set; }
    public TipoCompromisso TipoCompromisso { get; set; }
    public string Local { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public Contato? Contato { get; set; }

    public Compromisso(string assunto, DateTime dataOcorrencia, TimeSpan horaInicio, TimeSpan horaTermino,
        TipoCompromisso tipoCompromisso, string local, string link, Contato? contato)
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
    public Compromisso(Guid id, string assunto, DateTime dataOcorrencia, TimeSpan horaInicio, TimeSpan horaTermino,
        TipoCompromisso tipoCompromisso, string local, string link, Contato? contato) : this(assunto, dataOcorrencia, horaInicio, horaTermino, tipoCompromisso, local, link, contato)
    {
        Id = id;
    }

    public void Iniciar()
    {
        HoraInicio = DateTime.Now.TimeOfDay;
    }

    public void Terminar()
    {
        HoraTermino = DateTime.Now.TimeOfDay;
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