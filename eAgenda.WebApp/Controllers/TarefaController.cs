using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace eAgenda.WebApp.Controllers;

[Route("tarefas")]
public class TarefaController : Controller
{
    private readonly EAgendaDbContext contexto;
    private readonly IRepositorioTarefa repositorioTarefa;

    public TarefaController(EAgendaDbContext contexto, IRepositorioTarefa repositorioTarefa)
    {
        this.contexto = contexto;
        this.repositorioTarefa = repositorioTarefa;
    }

    public IActionResult Index(string? status, string? prioridade)
    {
        repositorioTarefa.AtualizarStatusRegistros();

        contexto.SaveChanges();

        List<Tarefa> tarefas = repositorioTarefa.SelecionarRegistros();

        if (!string.IsNullOrEmpty(status))
        {
            List<Tarefa> tarefasPorStatus = repositorioTarefa.SelecionarTarefasPorStatus(status);

            tarefas = [.. tarefas.IntersectBy(tarefasPorStatus.Select(x => x.Id), t => t.Id)];
        }

        if (!string.IsNullOrEmpty(prioridade))
        {
            List<Tarefa> tarefasPorPrioridade = repositorioTarefa.SelecionarTarefasPorPrioridade(prioridade);

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

        IDbContextTransaction transacao = this.contexto.Database.BeginTransaction();

        try
        {
            repositorioTarefa.CadastrarRegistro(novaTarefa);

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

        IDbContextTransaction transacao = this.contexto.Database.BeginTransaction();

        try
        {
            repositorioTarefa.EditarRegistro(id, tarefaEditada);

            this.contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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
        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioTarefa.ExcluirRegistro(id);

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

    [HttpGet, Route("/tarefas/{id:guid}/detalhes")]
    public IActionResult Detalhes(Guid id)
    {
        repositorioTarefa.AtualizarStatusRegistros();

        contexto.SaveChanges();

        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return View(detalhesTarefaVM);
    }

    [HttpGet, Route("/tarefas/{id:guid}/gerenciar-itens")]
    public IActionResult GerenciarItens(Guid id)
    {
        repositorioTarefa.AtualizarStatusRegistros();

        contexto.SaveChanges();

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
        ItemTarefa novoItem = new(adicionarItemVM.TituloItem, tarefaSelecionada) { Tarefa = tarefaSelecionada };

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

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            tarefaSelecionada.AdicionarItem(novoItem);
            contexto.Itens.Add(novoItem);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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

        IDbContextTransaction transacao = this.contexto.Database.BeginTransaction();

        try
        {
            tarefaSelecionada.RemoverItem(itemSelecionado);
            this.contexto.Itens.Remove(itemSelecionado);

            this.contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(contexto, new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/concluir-item/{idItem:guid}")]
    public IActionResult ConcluirItem(Guid id, Guid idItem)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;
        ItemTarefa itemSelecionado = repositorioTarefa.SelecionarItem(tarefaSelecionada, idItem)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            itemSelecionado.Concluir();

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

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

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            itemSelecionado.Reabrir();

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        GerenciarItensViewModel gerenciarItensVM = new(
            tarefaSelecionada,
            tarefaSelecionada.Itens);

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/concluir-tarefa")]
    public IActionResult ConcluirTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            tarefaSelecionada.Concluir();

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/reabrir-tarefa")]
    public IActionResult ReabrirTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            tarefaSelecionada.Reabrir();

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost, Route("/tarefas/{id:guid}/cancelar-tarefa")]
    public IActionResult CancelarTarefa(Guid id)
    {
        Tarefa tarefaSelecionada = repositorioTarefa.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            tarefaSelecionada.Cancelar();

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        DetalhesTarefaViewModel detalhesTarefaVM = tarefaSelecionada.ParaDetalhesVM();

        return RedirectToAction(nameof(Detalhes), new { id });
    }
}
