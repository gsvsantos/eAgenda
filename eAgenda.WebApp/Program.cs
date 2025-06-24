using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.Arquivos.ModuloTarefa;
using eAgenda.WebApp.ActionFilters;

namespace eAgenda.WebApp
{
#pragma warning disable RCS1102 // Make class static
    public class Program
#pragma warning restore RCS1102 // Make class static
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews((options) => options.Filters.Add<ValidarModeloAttribute>());
            builder.Services.AddScoped((IServiceProvider _) => new ContextoDados(true));
            builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefaEmArquivos>();

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapDefaultControllerRoute();
            app.Run();
        }
    }
}
