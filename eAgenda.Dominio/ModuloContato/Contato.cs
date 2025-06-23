using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCompromisso;
using System.Diagnostics.CodeAnalysis;

namespace eAgenda.Dominio.ModuloContato
{
    public class Contato : EntidadeBase<Contato>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cargo { get; set; }
        public string Empresa { get; set; }

        public List<Compromisso> Compromissos { get; set; } = [];

        [ExcludeFromCodeCoverage]
        public Contato() {}

        public Contato(string nome, string email, string telefone, string cargo, string empresa)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Cargo = cargo;
            Empresa = empresa;
        }

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

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                return "Email inválido";

            if (string.IsNullOrWhiteSpace(Telefone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
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
