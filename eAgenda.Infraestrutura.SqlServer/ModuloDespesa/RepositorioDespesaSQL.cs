using eAgenda.Dominio.ModuloDespesa;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloDespesa;

public class RepositorioDespesaSQL : IRepositorioDespesa
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Despesa novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        const string sqlCadastrar =
            @"INSERT INTO [TBDESPESA]
            (
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [DATAOCORRENCIA],
	            [VALOR],
	            [FORMAPAGAMENTO],
            )
            VALUES
            (
	            @ID,
	            @TITULO,
	            @DESCRICAO,
	            @DATAOCORRENCIA,
	            @VALOR,
	            @FORMAPAGAMENTO
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCadastro = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosDespesa(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Despesa registroEditado)
    {
        const string sqlEditar =
            @"UPDATE [TBDESPESA]
            SET
	            [TITULO] = @TITULO,
	            [DESCRICAO] = @DESCRICAO,
	            [DATAOCORRENCIA] = @DATAOCORRENCIA,
	            [VALOR] = @VALOR,
	            [FORMAPAGAMENTO] = @FORMAPAGAMENTO
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosDespesa(registroEditado, comandoEdicao);

        comandoEdicao.Parameters.AddWithValue("ID", idRegistro);

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        const string sqlExcluir =
            @"DELETE FROM [TBDESPESA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public Despesa? SelecionarRegistroPorId(Guid idRegistro)
    {
        const string sqlSelecionarPorId =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [DATAOCORRENCIA],
	            [VALOR],
	            [FORMAPAGAMENTO]
            FROM
	            [TBDESPESA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Despesa? despesa = null;

        while (leitor.Read())
        {
            despesa = ConverterParaDespesa(leitor);
        }

        conexaoComBanco.Close();

        return despesa;
    }

    public List<Despesa> SelecionarRegistros()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [DATAOCORRENCIA],
	            [VALOR],
	            [FORMAPAGAMENTO]
            FROM
	            [TBDESPESA]";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Despesa> despesas = [];

        while (leitor.Read())
        {
            despesas.Add(ConverterParaDespesa(leitor));
        }

        conexaoComBanco.Close();

        return despesas;
    }

    private Despesa ConverterParaDespesa(SqlDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["TITULO"])!,
            Convert.ToString(leitor["DESCRICAO"])!,
            Convert.ToDateTime(leitor["DATAOCORRENCIA"]),
            Convert.ToDecimal(leitor["VALOR"]),
            (MeiosPagamento)Convert.ToInt64(leitor["FORMAPAGAMENTO"])
            );
    }

    private void ConfigurarParametrosDespesa(Despesa novoRegistro, SqlCommand comandoCadastro)
    {
        comandoCadastro.Parameters.AddWithValue("ID", novoRegistro.Id);
        comandoCadastro.Parameters.AddWithValue("TITULO", novoRegistro.Titulo);
        comandoCadastro.Parameters.AddWithValue("DESCRICAO", novoRegistro.Descricao);
        comandoCadastro.Parameters.AddWithValue("DATAOCORRENCIA", novoRegistro.DataOcorrencia);
        comandoCadastro.Parameters.AddWithValue("VALOR", novoRegistro.Valor);
        comandoCadastro.Parameters.AddWithValue("FORMAPAGAMENTO", novoRegistro.FormaPagamento);
    }
}
