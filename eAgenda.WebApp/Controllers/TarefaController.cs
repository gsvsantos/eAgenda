using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers;

[Route("tarefas")]
public class TarefaController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioTarefa repositorioTarefa;

    public TarefaController(ContextoDados contextoDados, IRepositorioTarefa repositorioTarefa)
    {
        this.contextoDados = contextoDados;
        this.repositorioTarefa = repositorioTarefa;
    }

    public IActionResult Index(string? status, string? prioridade)
    {
        repositorioTarefa.AtualizarStatusRegistros();

        List<Tarefa> tarefas = repositorioTarefa.SelecionarRegistros();

        if (!string.IsNullOrEmpty(status))
        {
            List<Tarefa> tarefasPorStatus = status switch
            {
                "Pendente" => repositorioTarefa.SelecionarTarefasPendentes(),
                "EmAndamento" => repositorioTarefa.SelecionarTarefasEmAndamento(),
                "Concluida" => repositorioTarefa.SelecionarTarefasConcluidas(),
                "Cancelada" => repositorioTarefa.SelecionarTarefasCanceladas(),
                _ => tarefas
            };

            tarefas = [.. tarefas.IntersectBy(tarefasPorStatus.Select(x => x.Id), t => t.Id)];
        }

        if (!string.IsNullOrEmpty(prioridade))
        {
            List<Tarefa> tarefasPorPrioridade = prioridade switch
            {
                "Baixa" => repositorioTarefa.SelecionarTarefasPrioridadeBaixa(),
                "Media" => repositorioTarefa.SelecionarTarefasPrioridadeMedia(),
                "Alta" => repositorioTarefa.SelecionarTarefasPrioridadeAlta(),
                _ => tarefas
            };

            tarefas = [.. tarefas.IntersectBy(tarefasPorPrioridade.Select(x => x.Id), t => t.Id)];
        }

        VisualizarTarefasViewModel visualizarVM = new(tarefas);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        CadastrarTarefaViewModel cadastrarVM = new();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarTarefaViewModel cadastrarVM)
    {
        Tarefa novaTarefa = cadastrarVM.ParaEntidade();

        repositorioTarefa.CadastrarRegistro(novaTarefa);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id, string contexto = "Index")
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        EditarTarefaViewModel editarVM = new(
            id,
            tarefaSelecionada.Titulo,
            tarefaSelecionada.Prioridade,
            tarefaSelecionada.DataCriacao,
            tarefaSelecionada.Descricao);

        ViewBag.Contexto = contexto;

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarTarefaViewModel editarVM, string contexto)
    {
        Tarefa tarefaEditada = editarVM.ParaEntidade();

        repositorioTarefa.EditarRegistro(id, tarefaEditada);

        if (contexto == nameof(Detalhes))
            return RedirectToAction(nameof(Detalhes), new { id });

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        ExcluirTarefaViewModel excluirVM = new(
            id,
            tarefaSelecionada.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        repositorioTarefa.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/tarefas/{id:guid}/detalhes")]
    public IActionResult Detalhes(Guid id)
    {
        repositorioTarefa.AtualizarStatusRegistros();

        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return View(detalhesTarefaVM);
    }

    [HttpGet, Route("/tarefas/{id:guid}/gerenciar-itens")]
    public IActionResult GerenciarItens(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        List<ItemTarefa> itens = [.. tarefaSelecionada.Itens];

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            itens);

        return View(gerenciarItensVM);
    }

    [HttpPost, Route("/tarefas/{id:guid}/adicionar-item")]
    public IActionResult AdicionarItem(Guid id, AdicionarItemViewModel adicionarItemVM)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        ItemTarefa novoItem = new(adicionarItemVM.TituloItem, tarefaSelecionada);

        if (tarefaSelecionada.Itens.Any(i => i.Titulo == novoItem.Titulo))
        {
            ModelState.AddModelError("ConflitoItem", "A tarefa já contém este item!");
        }

        if (!ModelState.IsValid)
        {
            List<ItemTarefa> itens = [.. tarefaSelecionada.Itens];

            return View("GerenciarItens", new GerenciarItensViewModel(
            tarefaSelecionada,
            itens));
        }

        repositorioTarefa.AdicionarItem(novoItem);

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            tarefaSelecionada.Itens);

        return RedirectToAction(nameof(GerenciarItens), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/remover-item/{idItem:guid}")]
    public IActionResult RemoverItem(Guid id, Guid idItem, string contexto)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        ItemTarefa itemSelecionado = repositorioTarefa.SelecionarItem(tarefaSelecionada, idItem)!;

        repositorioTarefa.RemoverItem(itemSelecionado);

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            tarefaSelecionada.Itens);

        return RedirectToAction(contexto, new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/concluir-item/{idItem:guid}")]
    public IActionResult ConcluirItem(Guid id, Guid idItem)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        ItemTarefa itemSelecionado = repositorioTarefa.SelecionarItem(tarefaSelecionada, idItem)!;

        repositorioTarefa.ConcluirItem(itemSelecionado);

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            tarefaSelecionada.Itens);

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/reabrir-item/{idItem:guid}")]
    public IActionResult ReabrirItem(Guid id, Guid idItem)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        ItemTarefa itemSelecionado = repositorioTarefa.SelecionarItem(tarefaSelecionada, idItem)!;

        repositorioTarefa.ReabrirItem(itemSelecionado);

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            tarefaSelecionada.Itens);

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/concluir-tarefa")]
    public IActionResult ConcluirTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        repositorioTarefa.ConcluirItensTarefa(tarefaSelecionada);

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/reabrir-tarefa")]
    public IActionResult ReabrirTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        repositorioTarefa.ReabrirItensTarefa(tarefaSelecionada);

        contextoDados.Salvar();

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/cancelar-tarefa")]
    public IActionResult CancelarTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        tarefaSelecionada.Cancelar();
        repositorioTarefa.CancelarItensTarefa(tarefaSelecionada);

        contextoDados.Salvar();

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }
}
