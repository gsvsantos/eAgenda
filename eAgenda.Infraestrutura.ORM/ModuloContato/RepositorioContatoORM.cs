using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloContato;

public class RepositorioContatoORM : RepositorioBaseORM<Contato>, IRepositorioContato
{
    public RepositorioContatoORM(EAgendaDbContext contexto) : base(contexto) { }

    public override Contato? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Where(c => c.Id.Equals(idRegistro)).Include(c => c.Compromissos).FirstOrDefault();
    }

    public override List<Contato> SelecionarRegistros()
    {
        return [.. registros.Include(c => c.Compromissos)];
    }

    public bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null)
    {
        return registros.Any(x => (x.Email == email || x.Telefone == telefone)
            && (!ignorarId.HasValue || x.Id != ignorarId.Value));
    }

    public bool PossuiCompromissosVinculados(Guid id)
    {
        return registros.Any(c => c.Compromissos.Any(p => p.Contato!.Id == id));
    }
}
