using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApp.Helpers;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    [Route("contatos")]
    public class ContatoController : Controller
    {
        private readonly IRepositorioContato repositorioContato;

        public ContatoController(IRepositorioContato repositorioContato)
        {
            this.repositorioContato = repositorioContato;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            List<Contato> contatos = repositorioContato.SelecionarRegistros();
            VisualizarContatosViewModel visualizarVM = new(contatos);

            return View(visualizarVM);
        }

        [HttpGet("cadastrar")]
        public IActionResult Cadastrar()
        {
            CadastrarContatoViewModel cadastrarVM = new();

            return View(cadastrarVM);
        }

        [HttpPost("cadastrar")]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(CadastrarContatoViewModel cadastrarVM)
        {
            if (repositorioContato.SelecionarRegistros().Any(c => c.Email == cadastrarVM.Email)
                && repositorioContato.SelecionarRegistros().Any(c => c.Telefone == cadastrarVM.Telefone))
            {
                ModelState.AddModelError("CadastroUnico", "E-mail e Telefone já cadastrados!");
            }
            else if (repositorioContato.SelecionarRegistros().Any(c => c.Email == cadastrarVM.Email))
            {
                ModelState.AddModelError("CadastroUnico", "E-mail já cadastrado!");
            }
            else if (repositorioContato.SelecionarRegistros().Any(c => c.Telefone == cadastrarVM.Telefone))
            {
                ModelState.AddModelError("CadastroUnico", "Telefone já cadastrado!");
            }

            if (!ModelState.IsValid)
                return View(cadastrarVM);

            cadastrarVM.Telefone = TelefoneHelper.FormatarTelefone(cadastrarVM.Telefone);

            Contato contato = new(cadastrarVM.Nome, cadastrarVM.Email, cadastrarVM.Telefone, cadastrarVM.Cargo!, cadastrarVM.Empresa!);

            repositorioContato.CadastrarRegistro(contato);

            return RedirectToAction("Index");
        }

        [HttpGet("editar/{id:Guid}")]
        public IActionResult Editar(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;
            if (contato == null)
                return NotFound();
            EditarContatoViewModel editarVM = new(
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
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Guid id, EditarContatoViewModel editarVM)
        {
            if (repositorioContato.SelecionarRegistros().Any(c => c.Id != id && c.Email == editarVM.Email)
                && repositorioContato.SelecionarRegistros().Any(c => c.Id != id && c.Telefone == editarVM.Telefone))
            {
                ModelState.AddModelError("CadastroUnico", "E-mail e Telefone já cadastrados!");
            }
            else if (repositorioContato.SelecionarRegistros().Any(c => c.Id != id && c.Email == editarVM.Email))
            {
                ModelState.AddModelError("CadastroUnico", "E-mail já cadastrado!");
            }
            else if (repositorioContato.SelecionarRegistros().Any(c => c.Id != id && c.Telefone == editarVM.Telefone))
            {
                ModelState.AddModelError("CadastroUnico", "Telefone já cadastrado!");
            }

            if (!ModelState.IsValid)
                return View(editarVM);

            Contato contatoEditado = new(
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
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;
            if (contato == null)
                return NotFound();

            ExcluirContatoViewModel excluirVM = new(id, contato.Nome);

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
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;
            if (contato == null)
                return NotFound();
            DetalhesContatoViewModel detalhesVM = new(
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
