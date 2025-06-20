using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApp.Extensions;

namespace eAgenda.WebApp.Models;

public class FormularioCategoriaViewModel
{
    [Required(ErrorMessage = "O campo \"Titulo\" é obrigatório.")]
    [MinLength(3, ErrorMessage = "O campo \"Titulo\" precisa conter ao menos 3 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Titulo\" precisa conter no máximo 100 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O campo \"Despesas\" é obrigatório.")]
    public List<Despesa> Despesas { get; set; }
}

public class CadastrarCategoriaViewModel : FormularioCategoriaViewModel
{
    public CadastrarCategoriaViewModel() { }

    public CadastrarCategoriaViewModel(string titulo, List<Despesa> despesas) : this()
    {
        Titulo = titulo;
        Despesas = despesas;
    }
}

public class EditarCategoriaViewModel : FormularioCategoriaViewModel
{
    public Guid Id { get; set; }

    public EditarCategoriaViewModel() { }

    public EditarCategoriaViewModel(Guid id, string titulo, List<Despesa> despesas) : this()
    {
        Id = id;
        Titulo = titulo;
        Despesas = despesas;
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

public class DetalhesCategoriaViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public List<Despesa> Despesas { get; set; } = [];

    public DetalhesCategoriaViewModel(Guid id, string titulo, List<Despesa> despesas)
    {
        Id = id;
        Titulo = titulo;
        Despesas = despesas;
    }
}

public class SelecionarCategoriamViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }

    public SelecionarCategoriamViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}
