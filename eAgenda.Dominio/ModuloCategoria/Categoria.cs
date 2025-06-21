using System.Diagnostics.CodeAnalysis;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloDespesa;

namespace eAgenda.Dominio.ModuloCategoria;

public class Categoria : EntidadeBase<Categoria>
{
    public string Titulo { get; set; } = string.Empty;
    public List<Despesa> Despesas { get; set; } = [];

    [ExcludeFromCodeCoverage]
    public Categoria() { }
    public Categoria(string titulo) : this()
    {
        Titulo = titulo;
    }

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
