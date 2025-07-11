using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.WebApp.ORM;

public static class DatabaseOperations
{
    public static void ApplyMigrations(this IHost host)
    {
        using IServiceScope? scope = host.Services.CreateScope();
        EAgendaDbContext? db = scope.ServiceProvider.GetRequiredService<EAgendaDbContext>();

        db.Database.Migrate();
    }
}
