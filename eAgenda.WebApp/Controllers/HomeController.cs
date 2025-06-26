using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositorioTarefa repositorioTarefa;
        private readonly IRepositorioDespesa repositorioDespesa;
        private readonly IRepositorioContato repositorioContato;
        private readonly IRepositorioCompromisso repositorioCompromisso;
        private readonly IRepositorioCategoria repositorioCategoria;

        public HomeController(IRepositorioTarefa repositorioTarefa, IRepositorioDespesa repositorioDespesa,
            IRepositorioContato repositorioContato, IRepositorioCompromisso repositorioCompromisso,
            IRepositorioCategoria repositorioCategoria)
        {
            this.repositorioTarefa = repositorioTarefa;
            this.repositorioDespesa = repositorioDespesa;
            this.repositorioContato = repositorioContato;
            this.repositorioCompromisso = repositorioCompromisso;
            this.repositorioCategoria = repositorioCategoria;
        }

        public IActionResult Index()
        {
            HomeViewModel homeVM = new()
            {
                TotalCategorias = repositorioCategoria.SelecionarRegistros().Count,
                TotalDespesas = repositorioDespesa.SelecionarRegistros().Sum(d => d.Valor),
                UltimasDespesas = [.. repositorioDespesa.SelecionarRegistros()
                                .OrderByDescending(d => d.DataOcorrencia)
                                .Take(5)
                                .Select(d => $"{d.Titulo} - R$ {d.Valor}")],
                TotalTarefas = repositorioTarefa.SelecionarRegistros().Count,
                UltimasTarefas = [.. repositorioTarefa.SelecionarRegistros()
                                .OrderByDescending(t => t.DataCriacao)
                                .Take(5)
                                .Select(t => t.Titulo)],
                TotalCompromissos = repositorioCompromisso.SelecionarRegistros().Count,
                TotalContatos = repositorioContato.SelecionarRegistros().Count,
                ProximosCompromissos = [.. repositorioCompromisso.SelecionarRegistros()
                                .OrderByDescending(c => c.DataOcorrencia)
                                .Take(5)
                                .Select(c => $"{c.Assunto} - {c.TipoCompromisso.GetDisplayName()} - {c.DataOcorrencia.ToShortDateString()}")]
            };

            return View(homeVM);
        }

        [HttpGet("erro")]
        public IActionResult Erro()
        {
            return View();
        }
    }
}
