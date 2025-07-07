using eAgenda.Dominio.Compartilhado;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.Compartilhado;

public abstract class RepositorioBaseSQL<T> where T : EntidadeBase<T>
{
    protected readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";
    protected abstract string SqlCadastrar { get; }
    protected abstract string SqlEditar { get; }
    protected abstract string SqlExcluir { get; }
    protected abstract string SqlSelecionarPorId { get; }
    protected abstract string SqlSelecionarTodos { get; }

    public void CadastrarRegistro(T novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCadastro = new(SqlCadastrar, conexaoComBanco);

        ConfigurarParametrosRegistro(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoEdicao = new(SqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosRegistro(registroEditado, comandoEdicao);

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoExclusao = new(SqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public virtual T? SelecionarRegistroPorId(Guid idRegistro)
    {
        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(SqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        T? T = null;

        if (leitor.Read())
            T = ConverterParaRegistro(leitor);

        conexaoComBanco.Close();

        return T;
    }

    public virtual List<T> SelecionarRegistros()
    {
        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(SqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<T> Ts = [];

        while (leitor.Read())
        {
            Ts.Add(ConverterParaRegistro(leitor));
        }

        conexaoComBanco.Close();

        return Ts;
    }

    protected abstract T ConverterParaRegistro(SqlDataReader leitor);

    protected abstract void ConfigurarParametrosRegistro(T novoRegistro, SqlCommand comandoCadastro);
}
