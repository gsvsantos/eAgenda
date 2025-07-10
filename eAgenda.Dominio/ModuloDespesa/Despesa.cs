using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.ModuloCategoria;
using System.ComponentModel.DataAnnotations;

namespace eAgenda.Dominio.ModuloDespesa;

public class Despesa : EntidadeBase<Despesa>
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public MeiosPagamento FormaPagamento { get; set; }
    public List<Categoria> Categorias { get; set; } = [];

    public Despesa(string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento)
    {
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;
    }
    public Despesa(Guid id, string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento) : this(titulo, descricao, dataOcorrencia, valor, formaPagamento)
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;
    }
    private Despesa() { }

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
    [Display(Name = "Dinheiro")]
    Dinheiro = 0,
    [Display(Name = "Débito")]
    Debito = 1,
    [Display(Name = "Crédito")]
    Credito = 2,
    [Display(Name = "Parcelado")]
    Parcelado = 3
}
