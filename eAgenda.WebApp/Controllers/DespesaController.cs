using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
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

    public DespesaController(ContextoDados contextoDados, IRepositorioDespesa repositorioDespesa, IRepositorioCategoria repositorioCategoria)
    {
        this.contextoDados = contextoDados;
        this.repositorioDespesa = repositorioDespesa;
        this.repositorioCategoria = repositorioCategoria;
    }

    public IActionResult Index()
    {
        List<Despesa> registros = repositorioDespesa.SelecionarRegistros();

        VisualizarDespesasViewModel visualizarVM = new(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        CadastrarDespesaViewModel cadastrarVM = new(categorias);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarDespesaViewModel cadastrarVM)
    {
        List<Despesa> registros = repositorioDespesa.SelecionarRegistros();
        List<Categoria> categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        if (repositorioDespesa.SelecionarRegistros().Any(d => d.Titulo == cadastrarVM.Titulo))
        {
            ModelState.AddModelError("ConflitoCadastro", "Já existe uma Despesa registrada com este Título.");
        }
        else if (cadastrarVM.CategoriasSelecionadas.Count == 0)
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

        Despesa entidade = cadastrarVM.ParaEntidade();

        List<Guid>? categoriasSelecionadas = cadastrarVM.CategoriasSelecionadas;

        if (categoriasSelecionadas is not null)
        {
            foreach (Guid idCategoria in categoriasSelecionadas)
            {
                foreach (Categoria c in categoriasDisponiveis)
                {
                    if (idCategoria.Equals(c.Id))
                    {
                        entidade.AderirCategoria(c);
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
        Despesa registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id)!;
        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        EditarDespesaViewModel editarVM = new(
            id,
            registroSelecionado.Titulo,
            registroSelecionado.Descricao,
            registroSelecionado.DataOcorrencia,
            registroSelecionado.Valor,
            registroSelecionado.FormaPagamento,
            categorias,
            registroSelecionado.Categorias);

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarDespesaViewModel editarVM)
    {
        List<Despesa> registros = repositorioDespesa.SelecionarRegistros();
        List<Categoria> categoriasDisponiveis = repositorioCategoria.SelecionarRegistros();

        if (repositorioDespesa.SelecionarRegistros().Any(d => d.Id != id && d.Titulo == editarVM.Titulo))
        {
            ModelState.AddModelError("ConflitoCadastro", "Já existe uma Despesa registrada com este Título.");
        }
        else if (editarVM.CategoriasSelecionadas.Count == 0)
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

                editarVM.Categorias?.Add(selecionarVM);
            }

            return View(editarVM);
        }

        Despesa entidadeEditada = editarVM.ParaEntidade();

        repositorioDespesa.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Despesa registroSelecionado = repositorioDespesa.SelecionarRegistroPorId(id)!;

        ExcluirDespesaViewModel excluirVM = new(
            registroSelecionado.Id,
            registroSelecionado.Titulo);

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
        Despesa despesaSelecionada = repositorioDespesa.SelecionarRegistroPorId(id)!;
        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        GerenciarCategoriasViewModel gerenciarCategoriaVm = new(
            despesaSelecionada,
            categorias);

        return View(gerenciarCategoriaVm);
    }

    [HttpPost, Route("/despesas/{id:guid}/adicionar-categoria")]
    public IActionResult AdicionarCategoria(Guid id, AdicionarCategoriaViewModel adicionarCategoriaVm)
    {
        Despesa despesaSelecionada = repositorioDespesa.SelecionarRegistroPorId(id)!;
        Categoria categoriaSelecionado = repositorioCategoria.SelecionarRegistroPorId(adicionarCategoriaVm.IdCategoria)!;

        if (despesaSelecionada.Categorias.Any(i => i.Titulo == categoriaSelecionado!.Titulo))
        {
            ModelState.AddModelError("ConflitoCategoria", "A despesa já contém essa categoria!");
        }

        if (!ModelState.IsValid)
        {
            List<Categoria> categoriasDespesa = [.. despesaSelecionada.Categorias];

            return View("GerenciarCategorias",
                new GerenciarCategoriasViewModel(
                    despesaSelecionada,
                    categoriasDespesa));
        }

        despesaSelecionada.AderirCategoria(categoriaSelecionado);
        categoriaSelecionado.AderirDespesa(despesaSelecionada);

        contextoDados.Salvar();

        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        GerenciarCategoriasViewModel gerenciarCategoriaVm = new(
            despesaSelecionada,
            categorias);

        return RedirectToAction(nameof(GerenciarCategorias), new { id });
    }

    [HttpPost, Route("/despesas/{id:guid}/remover-categoria/{idCategoria:guid}")]
    public IActionResult RemoverCategoria(Guid id, Guid idCategoria)
    {
        Despesa despesaSelecionada = repositorioDespesa.SelecionarRegistroPorId(id)!;
        Categoria categoriaSelecionado = repositorioCategoria.SelecionarRegistroPorId(idCategoria)!;

        despesaSelecionada.RemoverCategoria(categoriaSelecionado);
        categoriaSelecionado.RemoverDespesa(despesaSelecionada);

        contextoDados.Salvar();

        List<Categoria> categorias = repositorioCategoria.SelecionarRegistros();

        GerenciarCategoriasViewModel gerenciarCategoriaVm = new(
            despesaSelecionada,
            categorias);

        return RedirectToAction(nameof(GerenciarCategorias), new { id });
    }
}
