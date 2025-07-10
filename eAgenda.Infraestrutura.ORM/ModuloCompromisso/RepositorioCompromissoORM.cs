using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloCompromisso;

public class RepositorioCompromissoORM : RepositorioBaseORM<Compromisso>, IRepositorioCompromisso
{
    public RepositorioCompromissoORM(EAgendaDbContext contexto) : base(contexto) { }

    public override Compromisso? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Where(c => c.Id.Equals(idRegistro)).Include(c => c.Contato).FirstOrDefault();
    }

    public override List<Compromisso> SelecionarRegistros()
    {
        return [.. registros.Include(c => c.Contato)];
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
