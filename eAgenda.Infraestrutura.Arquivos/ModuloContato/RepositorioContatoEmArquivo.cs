using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;

namespace eAgenda.Infraestrutura.Arquivos.ModuloContato
{
    public class RepositorioContatoEmArquivo : RepositorioBaseEmArquivo<Contato>, IRepositorioContato
    {
        private List<Contato> contatos = new List<Contato>();

        public RepositorioContatoEmArquivo(ContextoDados contexto) : base(contexto)
        {
        }

        public void Inserir(Contato contato)
        {
            if (ExistePorEmailOuTelefone(contato.Email, contato.Telefone))
                throw new Exception("Já existe um contato com este e-mail ou telefone");

            contatos.Add(contato);
        }

        public void Editar(Contato contato)
        {
            var existente = SelecionarPorId(contato.Id);
            if (existente == null)
                throw new Exception("Contato não encontrado");

            if (ExistePorEmailOuTelefone(contato.Email, contato.Telefone, contato.Id))
                throw new Exception("Já existe um contato com este e-mail ou telefone");

            existente.Nome = contato.Nome;
            existente.Email = contato.Email;
            existente.Telefone = contato.Telefone;
            existente.Cargo = contato.Cargo;
            existente.Empresa = contato.Empresa;
        }

        public void Excluir(Contato contato)
        {
            if (PossuiCompromissosVinculados(contato.Id))
                throw new Exception("Não é possível excluir o contato pois possui compromissos vinculados");

            contatos.Remove(contato);
        }

        public Contato SelecionarPorId(Guid id)
        {
            return contatos.FirstOrDefault(x => x.Id == id)!;
        }

        public List<Contato> SelecionarTodos()
        {
            return contatos;
        }

        public bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null)
        {
            return contatos.Any(x => (x.Email == email || x.Telefone == telefone)
                && (!ignorarId.HasValue || x.Id != ignorarId.Value));
        }

        public bool PossuiCompromissosVinculados(Guid id)
        {

            return false;
        }

        protected override List<Contato> ObterRegistros()
        {
            return contexto.Contatos;
        }
    }
}

