using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.Extensions;

public static class SqlExtensions
{
    public static void AdicionarValorNullavel(this SqlParameterCollection parameters, string name, object? value)
    {
        parameters.AddWithValue(name, value ?? DBNull.Value);
    }
}
