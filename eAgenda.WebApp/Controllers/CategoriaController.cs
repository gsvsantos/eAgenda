using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers;

[Route("categorias")]
public class CategoriaController : Controller
{
    private readonly IRepositorioCategoria repositorioCategoria;

    public CategoriaController(ContextoDados contextoDados, IRepositorioCategoria repositorioCategoria)
    {
        this.repositorioCategoria = repositorioCategoria;
    }

    public IActionResult Index()
    {
        var registros = repositorioCategoria.SelecionarRegistros();

        var visualizarVM = new VisualizarCategoriasViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarCategoriaViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarCategoriaViewModel cadastrarVM)
    {
        var registros = repositorioCategoria.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (item.Titulo.Equals(cadastrarVM.Titulo))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma Categoria registrado com este Titúlo.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        var entidade = cadastrarVM.ParaEntidade();

        repositorioCategoria.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public ActionResult Editar(Guid id)
    {
        var registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id);

        var editarVM = new EditarCategoriaViewModel(
            id,
            registroSelecionado!.Titulo
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarCategoriaViewModel editarVM)
    {
        var registros = repositorioCategoria.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (!item.Id.Equals(id) && item.Titulo.Equals(editarVM.Titulo))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma Categoria registrado com este Titúlo.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        var entidadeEditada = editarVM.ParaEntidade();

        repositorioCategoria.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirCategoriaViewModel(registroSelecionado!.Id, registroSelecionado.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        var categoria = repositorioCategoria.SelecionarRegistroPorId(id);

        if (categoria == null)
            return NotFound();

        if (categoria.Despesas.Any())
        {
            ModelState.AddModelError("ExclusaoVinculo", "Não é possível excluir esta categoria, pois há despesas vinculadas a ela.");

            var excluirVM = new ExcluirCategoriaViewModel(categoria.Id, categoria.Titulo);

            return View("Excluir", excluirVM);
        }

        repositorioCategoria.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }


    [HttpGet("despesas/{id:guid}")]
    public IActionResult Despesas(Guid id)
    {
        var categoria = repositorioCategoria.SelecionarRegistroPorId(id);

        if (categoria == null)
            return NotFound();

        var despesasVM = new DespesasCategoriaViewModel
        {
            Id = categoria.Id,
            Titulo = categoria.Titulo,
            Despesas = categoria.Despesas.Select(d => new DespesaCategoriaViewModel
            {
                Id = d.Id,
                Titulo = d.Titulo,
                Descricao = d.Descricao,
                DataOcorrencia = d.DataOcorrencia,
                Valor = d.Valor,
                FormaPagamento = d.FormaPagamento.ToString()
            }).ToList()
        };

        return View(despesasVM);
    }


}
