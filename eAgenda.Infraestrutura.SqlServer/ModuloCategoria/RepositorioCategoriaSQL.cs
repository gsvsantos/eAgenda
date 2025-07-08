using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

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
    private static string SqlSelecionarDespesas => @"SELECT
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

    public RepositorioCategoriaSQL(IDbConnection conexaoComBanco) : base(conexaoComBanco) { }

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

    private static Despesa ConverterParaDespesa(IDataReader leitor)
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
        conexaoComBanco.Open();

        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarDespesas;

        comandoSelecao.AdicionarParametro("CATEGORIA_ID", categoria.Id);

        IDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            Despesa despesa = ConverterParaDespesa(leitor);

            categoria.AderirDespesa(despesa);
        }

        conexaoComBanco.Close();
    }

    protected override Categoria ConverterParaRegistro(IDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["Titulo"])!);
    }

    protected override void ConfigurarParametrosRegistro(Categoria categoria, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", categoria.Id);
        comando.AdicionarParametro("TITULO", categoria.Titulo);
    }
}
