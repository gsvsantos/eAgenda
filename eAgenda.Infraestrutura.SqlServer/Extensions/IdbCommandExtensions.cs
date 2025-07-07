using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.Extensions;

public static class IdbCommandExtensions
{
    public static void AdicionarParametro(this IDbCommand comando, string nome, object valor)
    {
        IDbDataParameter parametro = comando.CreateParameter();
        parametro.ParameterName = nome;
        parametro.Value = valor;

        comando.Parameters.Add(parametro);
    }
}
