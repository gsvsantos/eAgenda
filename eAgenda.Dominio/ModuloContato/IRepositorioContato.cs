using eAgenda.Dominio.Compartilhado;
using System.Collections.Generic;

namespace eAgenda.Dominio.ModuloContato
{
    public interface IRepositorioContato : IRepositorio<Contato>
    {
        bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null);
        bool PossuiCompromissosVinculados(Guid id); 
    }
}