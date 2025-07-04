using eAgenda.Dominio.ModuloCategoria;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers;

[Route("categorias")]
public class CategoriaController : Controller
{
    private readonly IRepositorioCategoria repositorioCategoria;

    public CategoriaController(IRepositorioCategoria repositorioCategoria)
    {
        this.repositorioCategoria = repositorioCategoria;
    }

    public IActionResult Index()
    {
        List<Categoria> registros = repositorioCategoria.SelecionarRegistros();

        VisualizarCategoriasViewModel visualizarVM = new(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        CadastrarCategoriaViewModel cadastrarVM = new();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarCategoriaViewModel cadastrarVM)
    {
        foreach (Categoria item in repositorioCategoria.SelecionarRegistros())
        {
            if (item.Titulo.Equals(cadastrarVM.Titulo))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma Categoria registrado com este Titúlo.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        Categoria entidade = cadastrarVM.ParaEntidade();

        repositorioCategoria.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public ActionResult Editar(Guid id)
    {
        Categoria registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id)!;

        EditarCategoriaViewModel editarVM = new(
            id,
            registroSelecionado!.Titulo);

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarCategoriaViewModel editarVM)
    {
        foreach (Categoria item in repositorioCategoria.SelecionarRegistros())
        {
            if (!item.Id.Equals(id) && item.Titulo.Equals(editarVM.Titulo))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma Categoria registrado com este Titúlo.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        Categoria entidadeEditada = editarVM.ParaEntidade();

        repositorioCategoria.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Categoria registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id)!;

        ExcluirCategoriaViewModel excluirVM = new(
            registroSelecionado!.Id,
            registroSelecionado.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Categoria categoria = repositorioCategoria.SelecionarRegistroPorId(id)!;

        if (categoria == null)
            return NotFound();

        if (categoria.Despesas.Count != 0)
        {
            ModelState.AddModelError("ExclusaoVinculo", "Não é possível excluir esta categoria, pois há despesas vinculadas a ela.");

            ExcluirCategoriaViewModel excluirVM = new(
                categoria.Id,
                categoria.Titulo);

            return View("Excluir", excluirVM);
        }

        repositorioCategoria.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("despesas/{id:guid}")]
    public IActionResult Despesas(Guid id)
    {
        Categoria categoria = repositorioCategoria.SelecionarRegistroPorId(id)!;

        if (categoria == null)
            return NotFound();

        DetalhesCategoriaViewModel despesasVM = categoria.ParaDetalhesVM();

        return View(despesasVM);
    }
}
