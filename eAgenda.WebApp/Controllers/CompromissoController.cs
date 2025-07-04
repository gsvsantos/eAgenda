using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers;

[Route("compromissos")]
public class CompromissoController : Controller
{
    private readonly IRepositorioContato repositorioContato;
    private readonly IRepositorioCompromisso repositorioCompromisso;

    public CompromissoController(IRepositorioContato repositorioContato, IRepositorioCompromisso repositorioCompromisso)
    {
        this.repositorioContato = repositorioContato;
        this.repositorioCompromisso = repositorioCompromisso;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        List<Compromisso> compromissos = repositorioCompromisso.SelecionarRegistros();
        VisualizarCompromissosViewModel vm = new(compromissos);

        return View(vm);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        List<Contato> contatos = repositorioContato.SelecionarRegistros();
        CadastrarCompromissoViewModel viewModel = new(contatos);

        return View(viewModel);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarCompromissoViewModel vm)
    {
        Contato contato = null!;
        if (vm.ContatoId.HasValue)
            contato = repositorioContato.SelecionarRegistroPorId(vm.ContatoId.Value)!;

        Compromisso compromisso = vm.ParaEntidade(contato);

        if (repositorioCompromisso.TemConflito(compromisso))
            ModelState.AddModelError("ConflitoHorario", "Há compromisso marcado para esses horário!");
        else if (vm.HoraInicio > vm.HoraTermino)
            ModelState.AddModelError("ConflitoHorario", "A hora de início está após o horário final!");

        if (!ModelState.IsValid)
            return View(vm);

        repositorioCompromisso.CadastrarRegistro(compromisso);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id:Guid}")]
    public IActionResult Editar(Guid id)
    {
        Compromisso compromisso = repositorioCompromisso.SelecionarRegistroPorId(id)!;
        if (compromisso == null)
            return NotFound();

        List<Contato> contatos = repositorioContato.SelecionarRegistros();
        EditarCompromissoViewModel vm = new(compromisso, contatos);

        return View(vm);
    }

    [HttpPost("editar/{id:Guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarCompromissoViewModel vm)
    {
        Contato contato = null!;

        if (vm.ContatoId.HasValue)
            contato = repositorioContato.SelecionarRegistroPorId(vm.ContatoId.Value)!;

        Compromisso compromissoEditado = vm.ParaEntidade(contato);

        if (repositorioCompromisso.TemConflito(compromissoEditado))
            ModelState.AddModelError("ConflitoHorario", "Há compromisso marcado para esses horário!");
        else if (vm.HoraInicio > vm.HoraTermino)
            ModelState.AddModelError("ConflitoHorario", "A hora de início está após o horário final!");

        if (!ModelState.IsValid)
            return View(vm);

        repositorioCompromisso.EditarRegistro(id, compromissoEditado);

        return RedirectToAction("Index");
    }

    [HttpGet("excluir/{id:Guid}")]
    public IActionResult Excluir(Guid id)
    {
        Compromisso? compromisso = repositorioCompromisso.SelecionarRegistroPorId(id);
        if (compromisso == null)
            return NotFound();

        ExcluirCompromissoViewModel vm = new(id, compromisso.Assunto);
        return View(vm);
    }

    [HttpPost("excluir/{id:Guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        repositorioCompromisso.ExcluirRegistro(id);
        return RedirectToAction("Index");
    }

    [HttpGet("detalhes/{id:Guid}")]
    public IActionResult Detalhes(Guid id)
    {
        Compromisso? compromisso = repositorioCompromisso.SelecionarRegistroPorId(id);

        if (compromisso == null)
            return NotFound();

        DetalhesCompromissoViewModel vm = compromisso.ParaDetalhesVM();

        return View(vm);
    }
}
