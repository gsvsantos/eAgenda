using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.Arquivos.ModuloContato;
using eAgenda.WebApp.Helpers;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    [Route("contatos")]
    public class ContatoController : Controller
    {
        private readonly ContextoDados contextoDados;
        private readonly IRepositorioContato repositorioContato;

        public ContatoController()
        {
            contextoDados = new ContextoDados(true);
            repositorioContato = new RepositorioContatoEmArquivo(contextoDados);
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            List<Contato> contatos = repositorioContato.SelecionarRegistros();
            VisualizarContatosViewModel visualizarVM = new VisualizarContatosViewModel(contatos);

            return View(visualizarVM);
        }

        [HttpGet("cadastrar")]
        public IActionResult Cadastrar()
        {
            CadastrarContatoViewModel cadastrarVM = new CadastrarContatoViewModel();

            return View(cadastrarVM);
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar(CadastrarContatoViewModel cadastrarVM)
        {
            cadastrarVM.Telefone = TelefoneHelper.FormatarTelefone(cadastrarVM.Telefone);

            Contato contato = new Contato(cadastrarVM.Nome, cadastrarVM.Email, cadastrarVM.Telefone, cadastrarVM.Cargo!, cadastrarVM.Empresa!);

            repositorioContato.CadastrarRegistro(contato);

            return RedirectToAction("Index");
        }

        [HttpGet("editar/{id:Guid}")]
        public IActionResult Editar(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id);
            if (contato == null)
                return NotFound();
            EditarContatoViewModel editarVM = new EditarContatoViewModel(
                contato.Id,
                contato.Nome,
                contato.Telefone,
                contato.Email,
                contato.Cargo,
                contato.Empresa
            );
            return View(editarVM);
        }

        [HttpPost("editar/{id:Guid}")]
        public IActionResult Editar(Guid id, EditarContatoViewModel editarVM)
        {
            //string? validacao = contato.Validar();
            //if (!string.IsNullOrEmpty(validacao))
            //{
            //    ModelState.AddModelError("", validacao);
            //    return View(contato);
            //}

            //if (repositorioContato.ExistePorEmailOuTelefone(contato.Email, contato.Telefone, id))
            //{
            //    ModelState.AddModelError("", "Já existe um contato com este e-mail ou telefone");
            //    return View(contato);
            //}
            Contato contatoEditado = new Contato(
                editarVM.Nome,
                editarVM.Email,
                editarVM.Telefone,
                editarVM.Cargo!,
                editarVM.Empresa!
            );
            repositorioContato.EditarRegistro(id, contatoEditado);
            return RedirectToAction("Index");
        }

        [HttpGet("excluir/{id:Guid}")]
        public IActionResult Excluir(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id);
            if (contato == null)
                return NotFound();

            ExcluirContatoViewModel excluirVM = new ExcluirContatoViewModel(id, contato.Nome);

            return View(excluirVM);
        }

        [HttpPost("excluir/{id:Guid}")]
        public IActionResult ExcluirConfirmado(Guid id)
        {
            repositorioContato.ExcluirRegistro(id);
            return RedirectToAction("Index");
        }

        [HttpGet("detalhes/{id:Guid}")]
        public IActionResult Detalhes(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id);
            if (contato == null)
                return NotFound();
            DetalhesContatoViewModel detalhesVM = new DetalhesContatoViewModel(
                contato.Id,
                contato.Nome,
                contato.Email,
                contato.Telefone,
                contato.Cargo,
                contato.Empresa,
                contato.Compromissos);
            return View(detalhesVM);
        }
    }

}
