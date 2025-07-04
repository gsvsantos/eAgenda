using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloCategoria;

public class RepositorioCategoriaSQL : IRepositorioCategoria
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Categoria novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        const string sqlCadastrar =
            @"INSERT INTO [TBCATEGORIA]
            (
	            [ID],
	            [TITULO]
            )
            VALUES
            (
	            @ID,
	            @TITULO
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCadastro = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosCategoria(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Categoria registroEditado)
    {
        const string sqlEditar =
            @"UPDATE [TBCATEGORIA]
            SET
	            [TITULO] = @TITULO
            WHERE
	            [ID] = @ID";


        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosCategoria(registroEditado, comandoEdicao);

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        const string sqlExcluir =
            @"DELETE FROM [TBCATEGORIA]
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

    public Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {
        const string sqlSelecionarTodos =
            @"SELECT
                [ID],
                [TITULO]
            FROM
                [TBCATEGORIA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Categoria? categoria = null;

        if (leitor.Read())
            categoria = ConverterParaCategoria(leitor);

        if (categoria is not null)
            CarregarDespesas(categoria);

        conexaoComBanco.Close();

        return categoria;
    }

    public List<Categoria> SelecionarRegistros()
    {
        const string sqlSelecionarTodos =
            @"SELECT
                [ID],
                [TITULO]
            FROM
                [TBCATEGORIA]";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Categoria> categorias = [];

        while (leitor.Read())
        {
            categorias.Add(ConverterParaCategoria(leitor));
        }

        conexaoComBanco.Close();

        foreach (Categoria categoria in categorias)
            CarregarDespesas(categoria);

        return categorias;
    }

    private Categoria ConverterParaCategoria(SqlDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["Titulo"])!);
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

    private void ConfigurarParametrosCategoria(Categoria novoRegistro, SqlCommand comandoCadastro)
    {
        comandoCadastro.Parameters.AddWithValue("ID", novoRegistro.Id);
        comandoCadastro.Parameters.AddWithValue("TITULO", novoRegistro.Titulo);
    }

    private void CarregarDespesas(Categoria categoria)
    {
        const string sqlCarregarDespesas =
            @"SELECT
	            D.[ID],
	            D.[TITULO],
	            D.[DESCRICAO],
	            D.[DATAOCORRENCIA],
	            D.[VALOR],
	            D.[FORMAPAGAMENTO]
            FROM
	            [TBDESPESA] AS D
	            INNER JOIN [TBDESPESA_TBCATEGORIA] AS DC
            ON
	            D.[ID] = DC.[DESPESA_ID]
            WHERE
                DC.[CATEGORIA_ID] = @CATEGORIA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlCarregarDespesas, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("CATEGORIA_ID", categoria.Id);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            Despesa despesa = ConverterParaDespesa(leitor);

            categoria.AderirDespesa(despesa);
        }

        conexaoComBanco.Close();
    }
}
