using System.Diagnostics.CodeAnalysis;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCategoria;

namespace eAgenda.Dominio.ModuloDespesa;

public class Despesa : EntidadeBase<Despesa>
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public MeiosPagamento FormaPagamento { get; set; }
    public List<Categoria> Categorias { get; set; } = [];

    [ExcludeFromCodeCoverage]
    public Despesa() { }

    public Despesa(string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento) : this()
    {
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;
    }

    public void AderirCategoria(Categoria categoria)
    {
      Categorias.Add(categoria);
    }

    public void RemoverCategoria(Categoria categoria)
    {
        Categorias.Remove(categoria);
    }

    public override void AtualizarRegistro(Despesa registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Descricao = registroEditado.Descricao;
        DataOcorrencia = registroEditado.DataOcorrencia;
        Valor = registroEditado.Valor;
        FormaPagamento = registroEditado.FormaPagamento;
    }
}

public enum MeiosPagamento
{
    Dinheiro = 0,
    Debito = 1,
    Credito = 2,
    Parcelado = 3
}
