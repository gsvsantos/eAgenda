using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCompromisso;
using System.Text.RegularExpressions;

namespace eAgenda.Dominio.ModuloContato
{
    public class Contato : EntidadeBase<Contato>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Cargo { get; set; } = string.Empty;
        public string? Empresa { get; set; } = string.Empty;
        public List<Compromisso> Compromissos { get; set; } = [];

        public Contato(string nome, string email, string telefone, string? cargo, string? empresa)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Cargo = cargo;
            Empresa = empresa;
        }
        public Contato(Guid id, string nome, string email, string telefone, string? cargo, string? empresa) : this(nome, email, telefone, cargo, empresa)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Cargo = cargo;
            Empresa = empresa;
        }
        protected Contato() { }

        public void AdicionarCompromisso(Compromisso compromisso)
        {
            Compromissos.Add(compromisso);
        }
        public void RemoverCompromisso(Compromisso compromisso)
        {
            Compromissos.Remove(compromisso);
        }

        public string Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
                return "Nome deve ter entre 2 e 100 caracteres";

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
                return "Email inválido";

            if (string.IsNullOrWhiteSpace(Telefone) || !Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
                return "Telefone inválido. Formato esperado: (XX) XXXX-XXXX ou (XX) XXXXX-XXXX";

            return string.Empty;
        }

        public override void AtualizarRegistro(Contato registroEditado)
        {
            Nome = registroEditado.Nome;
            Email = registroEditado.Email;
            Telefone = registroEditado.Telefone;
            Cargo = registroEditado.Cargo;
            Empresa = registroEditado.Empresa;
        }
    }
}
