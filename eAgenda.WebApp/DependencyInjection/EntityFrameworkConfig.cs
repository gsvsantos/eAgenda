using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.WebApp.DependencyInjection;

public static class EntityFrameworkConfig
{
    public static void AddEntityFrameworkConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EAgendaDbContext>(options =>
        options.UseNpgsql(configuration["SQL_CONNECTION_STRING"]));
    }
}
