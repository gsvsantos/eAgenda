using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace eAgenda.WebApp.Controllers;

[Route("categorias")]
public class CategoriaController : Controller
{
    private readonly eAgendaDbContext contexto;
    private readonly IRepositorioCategoria repositorioCategoria;

    public CategoriaController(eAgendaDbContext contexto, IRepositorioCategoria repositorioCategoria)
    {
        this.contexto = contexto;
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

        Categoria novaCategoria = cadastrarVM.ParaEntidade();

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioCategoria.CadastrarRegistro(novaCategoria);

            contexto.SaveChanges();

            transacao.Commit();

        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioCategoria.EditarRegistro(id, entidadeEditada);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioCategoria.ExcluirRegistro(id);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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
