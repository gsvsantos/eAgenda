using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.ModuloCategoria;
using eAgenda.Infraestrutura.SQLServer.ModuloCompromisso;
using eAgenda.Infraestrutura.SQLServer.ModuloContato;
using eAgenda.Infraestrutura.SQLServer.ModuloDespesa;
using eAgenda.Infraestrutura.SQLServer.ModuloTarefa;
using eAgenda.WebApp.ActionFilters;
using eAgenda.WebApp.DependencyInjection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eAgenda.WebApp
{
#pragma warning disable RCS1102 // Make class static
    public class Program
#pragma warning restore RCS1102 // Make class static
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews((options) =>
            {
                options.Filters.Add<ValidarModeloAttribute>();
                options.Filters.Add<LogarAcaoAttribute>();
            });
            builder.Services.AddScoped<IDbConnection>(_ =>
            {
                const string connectionString =
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

                return new SqlConnection(connectionString);
            });
            builder.Services.AddScoped((IServiceProvider _) => new ContextoDados(true));
            builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoriaSQL>();
            builder.Services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoSQL>();
            builder.Services.AddScoped<IRepositorioContato, RepositorioContatoSQL>();
            builder.Services.AddScoped<IRepositorioDespesa, RepositorioDespesaSQL>();
            builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefaSQL>();

            builder.Services.AddSerilogConfig(builder.Logging);

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
