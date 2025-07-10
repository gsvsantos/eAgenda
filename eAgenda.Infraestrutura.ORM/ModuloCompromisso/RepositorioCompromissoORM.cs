using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Infraestrutura.ORM.Compartilhado;

namespace eAgenda.Infraestrutura.ORM.ModuloCompromisso;

public class RepositorioCompromissoORM : RepositorioBaseORM<Compromisso>, IRepositorioCompromisso
{
    public RepositorioCompromissoORM(EAgendaDbContext contexto) : base(contexto)
    {
    }

    public List<Compromisso> SelecionarCompromissosContato(Guid idRegistro)
    {
        throw new NotImplementedException();
    }

    public bool TemConflito(Compromisso compromisso)
    {
        throw new NotImplementedException();
    }
}
