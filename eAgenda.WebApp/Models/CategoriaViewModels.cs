using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Models;

public class FormularioCategoriaViewModel
{
    [Required(ErrorMessage = "O campo \"Titulo\" é obrigatório.")]
    [MinLength(2, ErrorMessage = "O campo \"Titulo\" precisa conter ao menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Titulo\" precisa conter no máximo 100 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O campo \"Despesas\" é obrigatório.")]
    public List<SelectListItem> Despesas { get; set; } = [];
}

public class CadastrarCategoriaViewModel : FormularioCategoriaViewModel
{
    public CadastrarCategoriaViewModel() { }

    public CadastrarCategoriaViewModel(string titulo) : this()
    {
        Titulo = titulo;
    }
}

public class EditarCategoriaViewModel : FormularioCategoriaViewModel
{
    public Guid Id { get; set; }

    public EditarCategoriaViewModel() { }

    public EditarCategoriaViewModel(Guid id, string titulo) : this()
    {
        Id = id;
        Titulo = titulo;
    }
}

public class ExcluirCategoriaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }

    public ExcluirCategoriaViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}

public class VisualizarCategoriasViewModel
{
    public List<DetalhesCategoriaViewModel> Registros { get; set; }

    public VisualizarCategoriasViewModel(List<Categoria> categorias)
    {
        Registros = new List<DetalhesCategoriaViewModel>();

        foreach (var c in categorias)
            Registros.Add(c.ParaDetalhesVM());
    }
}

public class DespesaCategoriaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public string FormaPagamento { get; set; }

    public DespesaCategoriaViewModel() { }

    public DespesaCategoriaViewModel(Guid id, string titulo, string descricao, DateTime dataOcorrencia, decimal valor, MeiosPagamento formaPagamento)
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia;
        Valor = valor;
        FormaPagamento = formaPagamento.ToString();
    }
}


public class DetalhesCategoriaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public List<DespesaCategoriaViewModel> Despesas { get; set; } = [];

    public DetalhesCategoriaViewModel() { }
    public DetalhesCategoriaViewModel(Guid id, string titulo, List<Despesa> despesas)
    {
        Id = id;
        Titulo = titulo;
        foreach (var despesa in despesas)
        {
            Despesas.Add(new DespesaCategoriaViewModel(despesa.Id, despesa.Titulo, despesa.Descricao, despesa.DataOcorrencia, despesa.Valor, despesa.FormaPagamento));
        }
    }
}
