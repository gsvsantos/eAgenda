using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloDespesa;

namespace eAgenda.Dominio.ModuloCategoria;

public class Categoria : EntidadeBase<Categoria>
{
    public string Titulo { get; set; } = string.Empty;
    public List<Despesa> Despesas { get; set; } = [];

    public Categoria(string titulo)
    {
        Titulo = titulo;
    }
    public Categoria(Guid id, string titulo) : this(titulo)
    {
        Id = id;
    }
    protected Categoria() { }

    public void AderirDespesa(Despesa despesa)
    {
        Despesas.Add(despesa);
    }

    public void RemoverDespesa(Despesa despesa)
    {
        Despesas.Remove(despesa);
    }
    public override void AtualizarRegistro(Categoria registroEditado)
    {
        Titulo = registroEditado.Titulo;
    }
}
