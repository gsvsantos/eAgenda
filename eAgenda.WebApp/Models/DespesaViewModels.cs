using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Models;

public class FormularioDespesaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo \"Titúlo\" é obrigatório.")]
    [MinLength(2, ErrorMessage = "O campo \"Titúlo\" precisa conter ao menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Titúlo\" precisa conter no máximo 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo \"Descrição\" é obrigatório.")]
    [MinLength(3, ErrorMessage = "O campo \"Descrição\" precisa conter ao menos 3 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Descrição\" precisa conter no máximo 100 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo \"Data de ocorrencia\" é obrigatória.")]
    public DateTime DataOcorrencia { get; set; }

    [Required(ErrorMessage = "O campo \"Valor\" é obrigatório.")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "O valor não pode ser negativo.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O campo \"Forma Pagamento\" é obrigatório.")]
    public MeiosPagamento FormaPagamento { get; set; }

    [Required(ErrorMessage = "O campo \"Categorias Selecionadas\" é necessita de ao menos um valor preenchido.")]
    public List<SelectListItem>? Categorias { get; set; } = [];

    [Required(ErrorMessage = "Selecione ao menos uma categoria para a Despesa.")]
    public List<Guid> CategoriasSelecionadas { get; set; } = [];
}

public class CadastrarDespesaViewModel : FormularioDespesaViewModel
{
    public CadastrarDespesaViewModel() { }

    public CadastrarDespesaViewModel(List<Categoria> categoriasDisponiveis) : this()
    {
        foreach (var c in categoriasDisponiveis)
        {
            var selecionarVM = new SelectListItem(c.Titulo, c.Id.ToString());

            Categorias?.Add(selecionarVM);
        }
    }
}

public class EditarDespesaViewModel : FormularioDespesaViewModel
{
    public EditarDespesaViewModel() { }

    public EditarDespesaViewModel(Guid id, string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento, List<Categoria> categoriasDisponiveis, List<Categoria> categoriasSelecionadas) : this()
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;
        foreach (var c in categoriasDisponiveis)
        {
            Categorias?.Add(new SelectListItem()
            {
                Text = c.Titulo,
                Value = c.Id.ToString()
            });
        }
        CategoriasSelecionadas = categoriasSelecionadas.ConvertAll(c => c.Id);
    }
}

public class ExcluirDespesaViewModel : FormularioDespesaViewModel
{
    public ExcluirDespesaViewModel() { }
    public ExcluirDespesaViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}

public class VisualizarDespesasViewModel
{
    public List<DetalhesDespesaViewModel> Registros { get; set; } = [];

    public VisualizarDespesasViewModel(List<Despesa> despesas)
    {
        foreach (var d in despesas)
            Registros.Add(d.ParaDetalhesVM());
    }
}


public class GerenciarCategoriasViewModel
{
    public DetalhesDespesaViewModel Despesa { get; set; } = null!;
    public List<SelectListItem> Categorias { get; set; } = [];
    public GerenciarCategoriasViewModel() { }

    public GerenciarCategoriasViewModel(Despesa despesa, List<Categoria> categorias) : this()
    {
        Despesa = despesa.ParaDetalhesVM();

        foreach (var p in categorias)
        {
            var selectItem = new SelectListItem(p.Titulo, p.Id.ToString());

            Categorias.Add(selectItem);
        }
    }
}

public class AdicionarCategoriaViewModel
{
    public Guid IdCategoria { get; set; }
}

public class CategoriaDespesaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;

    public CategoriaDespesaViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}

public class DetalhesDespesaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public MeiosPagamento FormaPagamento { get; set; }
    public List<CategoriaDespesaViewModel> Categorias { get; set; } = [];

    public DetalhesDespesaViewModel(Guid id, string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento, List<Categoria> categorias)
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento;

        foreach (var categoria in categorias)
        {
            Categorias.Add(new CategoriaDespesaViewModel(categoria.Id, categoria.Titulo));
        }
    }
}