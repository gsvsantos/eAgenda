using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infra.Dados.Arquivo.ModuloCompromisso;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.Arquivos.ModuloCategoria;
using eAgenda.Infraestrutura.Arquivos.ModuloContato;
using eAgenda.Infraestrutura.Arquivos.ModuloDespesa;
using eAgenda.Infraestrutura.Arquivos.ModuloTarefa;
using eAgenda.WebApp.Extensions;
using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoDados contextoDados;
        private readonly IRepositorioTarefa repositorioTarefa;
        private readonly IRepositorioDespesa repositorioDespesa;
        private readonly IRepositorioContato repositorioContato;
        private readonly IRepositorioCompromisso repositorioCompromisso;
        private readonly IRepositorioCategoria repositorioCategoria;

        public HomeController()
        {
            contextoDados = new(true);
            repositorioContato = new RepositorioContatoEmArquivo(contextoDados);
            repositorioCompromisso = new RepositorioCompromissoEmArquivo(contextoDados);
            repositorioCategoria = new RepositorioCatergoriaEmArquivo(contextoDados);
            repositorioDespesa = new RepositorioDespesaEmArquivo(contextoDados);
            repositorioTarefa = new RepositorioTarefaEmArquivos(contextoDados);
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
    }
}
