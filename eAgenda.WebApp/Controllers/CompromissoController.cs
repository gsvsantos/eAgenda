using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Infra.Dados.Arquivo.ModuloCompromisso;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.Arquivos.ModuloContato;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers;

[Route("compromissos")]
public class CompromissoController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioContato repositorioContato;
    private readonly IRepositorioCompromisso repositorioCompromisso;

    public CompromissoController()
    {
        contextoDados = new ContextoDados(true);
        repositorioContato = new RepositorioContatoEmArquivo(contextoDados);
        repositorioCompromisso = new RepositorioCompromissoEmArquivo(contextoDados);
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        List<Compromisso> compromissos = repositorioCompromisso.SelecionarRegistros();
        var vm = new VisualizarCompromissosViewModel(compromissos);
        
        return View(vm);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var contatos = repositorioContato.SelecionarRegistros();
        var viewModel = new CadastrarCompromissoViewModel(contatos);

        return View(viewModel);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarCompromissoViewModel vm)
    {
        Contato? contato = null!;
        if (vm.ContatoId != Guid.Empty)
        {
            contato = repositorioContato.SelecionarRegistroPorId(vm.ContatoId);
        }
        var compromisso = new Compromisso(
            vm.Assunto,
            vm.DataOcorrencia,
            vm.HoraInicio,
            vm.HoraTermino,
            vm.TipoCompromisso,
            vm.Local!,
            vm.Link!,
            contato
        );

        var erros = compromisso.Validar();

        if (repositorioCompromisso.TemConflito(compromisso))
            erros.Add("Já existe um compromisso nesse horário.");

        repositorioCompromisso.CadastrarRegistro(compromisso);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id:Guid}")]
    public IActionResult Editar(Guid id)
    {
        var compromisso = repositorioCompromisso.SelecionarRegistroPorId(id);
        if (compromisso == null)
            return NotFound();

        var contatos = repositorioContato.SelecionarRegistros();
        var vm = new EditarCompromissoViewModel(compromisso, contatos);

        return View(vm);
    }

    [HttpPost("editar/{id:Guid}")]
    public IActionResult Editar(Guid id, EditarCompromissoViewModel vm)
    {
        var contato = repositorioContato.SelecionarRegistroPorId(vm.ContatoId);

        var compromissoEditado = new Compromisso(
            vm.Assunto,
            vm.DataOcorrencia,
            vm.HoraInicio,
            vm.HoraTermino,
            vm.TipoCompromisso,
            vm.Local,
            vm.Link,
            contato
        ); 

        var erros = compromissoEditado.Validar();

        if (repositorioCompromisso.TemConflito(compromissoEditado))
            erros.Add("Já existe um compromisso nesse horário.");
        repositorioCompromisso.EditarRegistro(id, compromissoEditado);

        return RedirectToAction("Index");
    }


    [HttpGet("excluir/{id:Guid}")]
    public IActionResult Excluir(Guid id)
    {
        var compromisso = repositorioCompromisso.SelecionarRegistroPorId(id);
        if (compromisso == null)
            return NotFound();

        var vm = new ExcluirCompromissoViewModel(id, compromisso.Assunto);
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
        var compromisso = repositorioCompromisso.SelecionarRegistroPorId(id);
        if (compromisso == null)
            return NotFound();
        var vm = new DetalhesCompromissoViewModel(id,compromisso.Assunto, compromisso.DataOcorrencia, compromisso.HoraInicio, compromisso.HoraTermino, compromisso.TipoCompromisso,
            compromisso.Local, compromisso.Link, compromisso.Contato);
        return View(vm);
    }
}


