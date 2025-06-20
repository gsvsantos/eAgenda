using System.Diagnostics.CodeAnalysis;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCategoria;

namespace eAgenda.Dominio.ModuloDespesa;

public class Despesa : EntidadeBase<Despesa>
{
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public MeiosPagamento FormaPagamento { get; set; }
    public List<Categoria> Categorias { get; set; } = [];

    [ExcludeFromCodeCoverage]
    public Despesa() { }

    public Despesa(string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento) : this()
    {
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;
    }

    public enum MeiosPagamento
    {
        Dinheiro = 0,
        Debito = 1,
        Credito = 2,
        Parcelado = 3
    }

    public void AderirCategorias(List<Categoria> categorias)
    {
        foreach (Categoria c in categorias)
        {
            Categorias.Add(c);
        }
    }

    public void RemoverCategorias(List<Categoria> categorias)
    {
        foreach (Categoria c in categorias)
        {
            Categorias.Remove(c);
        }
    }

    public override void AtualizarRegistro(Despesa registroEditado)
    {
        Descricao = registroEditado.Descricao;
        DataOcorrencia = registroEditado.DataOcorrencia;
        Valor = registroEditado.Valor;
        FormaPagamento = registroEditado.FormaPagamento;
    }
}
