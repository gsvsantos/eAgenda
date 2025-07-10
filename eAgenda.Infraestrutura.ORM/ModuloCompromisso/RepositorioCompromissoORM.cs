using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Infraestrutura.ORM.Compartilhado;

namespace eAgenda.Infraestrutura.ORM.ModuloCompromisso;

public class RepositorioCompromissoORM : RepositorioBaseORM<Compromisso>, IRepositorioCompromisso
{
    public RepositorioCompromissoORM(EAgendaDbContext contexto) : base(contexto) { }

    public List<Compromisso> SelecionarCompromissosContato(Guid idRegistro)
    {
        return registros.Where(comp =>
            comp.Contato!.Id.Equals(idRegistro))
            .ToList();
    }

    public bool TemConflito(Compromisso compromisso)
    {
        return registros.Any(c =>
            c.Id != compromisso.Id && c.DataOcorrencia.Equals(compromisso.DataOcorrencia) &&
            (
                (compromisso.HoraInicio >= c.HoraInicio && compromisso.HoraInicio < c.HoraTermino) ||
                (compromisso.HoraTermino > c.HoraInicio && compromisso.HoraTermino <= c.HoraTermino) ||
                (compromisso.HoraInicio <= c.HoraInicio && compromisso.HoraTermino >= c.HoraTermino)
               )
           );
    }
}
