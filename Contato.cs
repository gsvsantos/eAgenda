namespace eAgenda.Dominio.ModuloContato
{
    public class Contato
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cargo { get; set; }
        public string Empresa { get; set; }

        public Contato()
        {
            Id = Guid.NewGuid();
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
    }
}
