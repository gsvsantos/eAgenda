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
using eAgenda.WebApp.ActionFilters;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace eAgenda.WebApp
{
#pragma warning disable RCS1102 // Make class static
    public class Program
#pragma warning restore RCS1102 // Make class static
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews((options) => options.Filters.Add<ValidarModeloAttribute>());
            builder.Services.AddScoped((IServiceProvider _) => new ContextoDados(true));
            builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoriaEmArquivo>();
            builder.Services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoEmArquivo>();
            builder.Services.AddScoped<IRepositorioContato, RepositorioContatoEmArquivo>();
            builder.Services.AddScoped<IRepositorioDespesa, RepositorioDespesaEmArquivo>();
            builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefaEmArquivos>();

            string caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string caminhoArquivo = Path.Combine(caminhoAppData, "eAgenda", "erro.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(new CompactJsonFormatter(), caminhoArquivo, LogEventLevel.Error)
                .CreateLogger();

            builder.Logging.ClearProviders();

            builder.Services.AddSerilog(Log.Logger);

            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/erro");
            else
                app.UseDeveloperExceptionPage();

            app.UseAntiforgery();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapDefaultControllerRoute();
            app.Run();
        }
    }
}
