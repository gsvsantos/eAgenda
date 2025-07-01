using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloContato
{
    public interface IRepositorioContato : IRepositorio<Contato>
    {
        bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null);
        bool PossuiCompromissosVinculados(Guid id);
    }
}