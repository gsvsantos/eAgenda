using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.ORM.Compartilhado;

namespace eAgenda.Infraestrutura.ORM.ModuloConta;

public class RepositorioContatoORM : RepositorioBaseORM<Contato>, IRepositorioContato
{
    public RepositorioContatoORM(EAgendaDbContext contexto) : base(contexto)
    {
    }

    public bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null)
    {
        throw new NotImplementedException();
    }

    public bool PossuiCompromissosVinculados(Guid id)
    {
        throw new NotImplementedException();
    }
}
