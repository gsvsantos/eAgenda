using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;
using eAgenda.Infraestrutura.ORM.ModuloCategoria;
using eAgenda.Infraestrutura.ORM.ModuloCompromisso;
using eAgenda.Infraestrutura.ORM.ModuloContato;
using eAgenda.Infraestrutura.ORM.ModuloDespesa;
using eAgenda.Infraestrutura.ORM.ModuloTarefa;
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
                string? connectionString = builder.Configuration["SQL_CONNECTION_STRING"];

                return new SqlConnection(connectionString);
            });
            builder.Services.AddScoped((IServiceProvider _) => new ContextoDados(true));
            builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoriaORM>();
            builder.Services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoORM>();
            builder.Services.AddScoped<IRepositorioContato, RepositorioContatoORM>();
            builder.Services.AddScoped<IRepositorioDespesa, RepositorioDespesaORM>();
            builder.Services.AddScoped<IRepositorioTarefa, RepositorioTarefaORM>();

            builder.Services.AddEntityFrameworkConfig(builder.Configuration);
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
