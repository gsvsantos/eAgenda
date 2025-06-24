using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.Arquivos.ModuloCategoria;
using eAgenda.Infraestrutura.Arquivos.ModuloDespesa;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAgenda.WebApp.Controllers;

[Route("despesas")]
public class DespesaController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioDespesa repositorioDespesa;
    private readonly IRepositorioCategoria repositorioCategoria;

    public DespesaController()
    {
        contextoDados = new ContextoDados(true);
        repositorioDespesa = new RepositorioDespesaEmArquivo(contextoDados);
        repositorioCategoria = new RepositorioCatergoriaEmArquivo(contextoDados);
    }

    public IActionResult Index()
    {
        var registros = repositorioDespesa.SelecionarRegistros();

        var visualizarVM = new VisualizarDespesasViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        var cadastrarVM = new CadastrarDespesaViewModel(categorias);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarDespesaViewModel cadastrarVM)
    {
        var registros = repositorioDespesa.SelecionarRegistros();
        var categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        if (repositorioDespesa.SelecionarRegistros().Any(d => d.Titulo == cadastrarVM.Titulo))
        {
            ModelState.AddModelError("ConflitoCadastro", "Já existe uma Despesa registrada com este Título.");
        }
        else if (cadastrarVM.CategoriasSelecionadas.Count <= 0)
        {
            ModelState.AddModelError("ConflitoCadastro", "Selecione ao menos uma categoria.");
        }

        if (!ModelState.IsValid)
        {
            foreach (Categoria categoria in categoriasDisponiveis)
            {
                SelectListItem selecionarVM = new()
                {
                    Text = categoria.Titulo,
                    Value = categoria.Id.ToString()
                };

                cadastrarVM.Categorias?.Add(selecionarVM);
            }

            return View(cadastrarVM);
        }

        var entidade = cadastrarVM.ParaEntidade();

        var categoriasSelecionadas = cadastrarVM.CategoriasSelecionadas;

        if (categoriasSelecionadas is not null)
        {
            foreach (var cs in categoriasSelecionadas)
            {
                foreach (var cd in categoriasDisponiveis)
                {
                    if (cs.Equals(cd.Id))
                    {
                        entidade.AderirCategoria(cd);
                        break;
                    }
                }
            }
        }

        repositorioDespesa.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public ActionResult Editar(Guid id)
    {
        var registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id);

        var editarVM = new EditarDespesaViewModel(
            id,
            registroSelecionado.Titulo,
            registroSelecionado.Descricao,
            registroSelecionado.DataOcorrencia,
            registroSelecionado.Valor,
            registroSelecionado.FormaPagamento
        );
        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarDespesaViewModel editarVM)
    {
        var registros = repositorioDespesa.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (!item.Id.Equals(id) && item.Titulo.Equals(editarVM.Titulo))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma Despesa registrado com este Titúlo.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        var entidadeEditada = editarVM.ParaEntidade();

        repositorioDespesa.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirDespesaViewModel(registroSelecionado.Id, registroSelecionado.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        repositorioDespesa.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/despesas/{id:guid}/gerenciar-categorias")]
    public IActionResult GerenciarCategorias(Guid id)
    {
        var despesaSelecionada = repositorioDespesa.SelecionarPorId(id);
        var categorias = repositorioCategoria.SelecionarRegistros();

        var gerenciarCategoriaVm = new GerenciarCategoriasViewModel(despesaSelecionada, categorias);

        return View(gerenciarCategoriaVm);
    }

    [HttpPost, Route("/despesas/{id:guid}/adicionar-categoria")]
    public IActionResult AdicionarCategoria(Guid id, AdicionarCategoriaViewModel adicionarCategoriaVm)
    {
        var despesaSelecionada = repositorioDespesa.SelecionarPorId(id);
        var categoriaSelecionado = repositorioCategoria.SelecionarRegistroPorId(adicionarCategoriaVm.IdCategoria);

        if (despesaSelecionada.Categorias.Any(i => i.Titulo == categoriaSelecionado.Titulo))
        {
            ModelState.AddModelError("ConflitoCategoria", "A despesa já contém essa categoria!");
        }

        if (!ModelState.IsValid)
        {
            List<Categoria> categoriasDespesa = [.. despesaSelecionada.Categorias];

            return View("GerenciarCategorias", new GerenciarCategoriasViewModel(
            despesaSelecionada,
            categoriasDespesa));
        }

        despesaSelecionada.AderirCategoria(categoriaSelecionado);
        categoriaSelecionado.AderirDespesa(despesaSelecionada);

        contextoDados.Salvar();

        var categorias = repositorioCategoria.SelecionarRegistros();

        var gerenciarCategoriaVm = new GerenciarCategoriasViewModel(despesaSelecionada, categorias);

        return RedirectToAction(nameof(GerenciarCategorias), new { id });
    }

    [HttpPost, Route("/despesas/{id:guid}/remover-categoria/{idCategoria:guid}")]
    public IActionResult RemoverCategoria(Guid id, Guid idCategoria)
    {
        var despesaSelecionada = repositorioDespesa.SelecionarPorId(id);
        var categoriaSelecionado = repositorioCategoria.SelecionarRegistroPorId(idCategoria);

        despesaSelecionada.RemoverCategoria(categoriaSelecionado);
        categoriaSelecionado.RemoverDespesa(despesaSelecionada);

        contextoDados.Salvar();

        var categorias = repositorioCategoria.SelecionarRegistros();

        var gerenciarCategoriaVm = new GerenciarCategoriasViewModel(despesaSelecionada, categorias);

        return RedirectToAction(nameof(GerenciarCategorias), new { id });
    }
}
