using System.Collections.Generic;

namespace eAgenda.Dominio.ModuloContato
{
    public interface IRepositorioContatos
    {
        void Inserir(Contato contato);
        void Editar(Contato contato);
        void Excluir(Contato contato);
        Contato SelecionarPorId(Guid id);
        List<Contato> SelecionarTodos();
        bool ExistePorEmailOuTelefone(string email,string telefone, Guid? ignorarId = null);
        bool PossuiCompromissoVinculados(Guid id);
    }
}