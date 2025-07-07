using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloCategoria;

public class RepositorioCategoriaSQL : RepositorioBaseSQL<Categoria>, IRepositorioCategoria
{
    protected override string SqlCadastrar => @"INSERT INTO [TBCATEGORIA]
            (
	            [ID],
	            [TITULO]
            )
            VALUES
            (
	            @ID,
	            @TITULO
            )";

    protected override string SqlEditar => @"UPDATE [TBCATEGORIA]
            SET
	            [TITULO] = @TITULO
            WHERE
	            [ID] = @ID";

    protected override string SqlExcluir => @"DELETE FROM [TBCATEGORIA]
            WHERE
	            [ID] = @ID";

    protected override string SqlSelecionarPorId => @"SELECT
                [ID],
                [TITULO]
            FROM
                [TBCATEGORIA]
            WHERE
                [ID] = @ID";

    protected override string SqlSelecionarTodos => @"SELECT
                [ID],
                [TITULO]
            FROM
                [TBCATEGORIA]";
    protected string SqlSelecionarDespesas => @"SELECT
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

    public override Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {
        Categoria? categoria = base.SelecionarRegistroPorId(idRegistro);

        if (categoria is not null)
            CarregarDespesas(categoria);

        return categoria;
    }

    public override List<Categoria> SelecionarRegistros()
    {
        List<Categoria> categorias = base.SelecionarRegistros();

        foreach (Categoria categoria in categorias)
            CarregarDespesas(categoria);

        return categorias;
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

    protected override Categoria ConverterParaRegistro(SqlDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["Titulo"])!);
    }

    protected override void ConfigurarParametrosRegistro(Categoria novoRegistro, SqlCommand comandoCadastro)
    {
        comandoCadastro.Parameters.AddWithValue("ID", novoRegistro.Id);
        comandoCadastro.Parameters.AddWithValue("TITULO", novoRegistro.Titulo);
    }
}
