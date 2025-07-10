using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Helpers;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace eAgenda.WebApp.Controllers
{
    [Route("contatos")]
    public class ContatoController : Controller
    {
        private readonly EAgendaDbContext contexto;
        private readonly IRepositorioContato repositorioContato;
        private readonly IRepositorioCompromisso repositorioCompromisso;

        public ContatoController(EAgendaDbContext contexto, IRepositorioContato repositorioContato, IRepositorioCompromisso repositorioCompromisso)
        {
            this.contexto = contexto;
            this.repositorioContato = repositorioContato;
            this.repositorioCompromisso = repositorioCompromisso;
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

            Contato contato = cadastrarVM.ParaEntidade();

            IDbContextTransaction transacao = contexto.Database.BeginTransaction();

            try
            {
                repositorioContato.CadastrarRegistro(contato);

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
                contato.Cargo!,
                contato.Empresa!);

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

            Contato contatoEditado = editarVM.ParaEntidade();

            IDbContextTransaction transacao = contexto.Database.BeginTransaction();

            try
            {
                repositorioContato.EditarRegistro(id, contatoEditado);

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

        [HttpGet("excluir/{id:Guid}")]
        public IActionResult Excluir(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;

            if (contato == null)
                return NotFound();

            ExcluirContatoViewModel excluirVM = new(
                id,
                contato.Nome);

            return View(excluirVM);
        }

        [HttpPost("excluir/{id:Guid}")]
        public IActionResult ExcluirConfirmado(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;

            if (repositorioContato.PossuiCompromissosVinculados(id))
            {
                ModelState.AddModelError("ExclusaoVinculo", "Não é possível excluir este contato, pois há compromissos vinculados a ele.");

                ExcluirContatoViewModel excluirVM = new(
                contato.Id,
                    contato.Nome);

                return View("Excluir", excluirVM);
            }

            IDbContextTransaction transacao = contexto.Database.BeginTransaction();

            try
            {
                repositorioContato.ExcluirRegistro(id);

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

        [HttpGet("detalhes/{id:Guid}")]
        public IActionResult Detalhes(Guid id)
        {
            Contato contato = repositorioContato.SelecionarRegistroPorId(id)!;
            contato.Compromissos = repositorioCompromisso.SelecionarCompromissosContato(id);

            if (contato == null)
                return NotFound();

            DetalhesContatoViewModel detalhesVM = contato.ParaDetalhesVM();

            return View(detalhesVM);
        }
    }
}
