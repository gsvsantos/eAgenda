using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.ModuloDespesa;

public class RepositorioDespesaSQL : RepositorioBaseSQL<Despesa>, IRepositorioDespesa
{
    protected override string SqlCadastrar => @"INSERT INTO [TBDESPESA]
            (
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [DATAOCORRENCIA],
	            [VALOR],
	            [FORMAPAGAMENTO]
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
    protected override string SqlEditar => @"UPDATE [TBDESPESA]
            SET
	            [TITULO] = @TITULO,
	            [DESCRICAO] = @DESCRICAO,
	            [DATAOCORRENCIA] = @DATAOCORRENCIA,
	            [VALOR] = @VALOR,
	            [FORMAPAGAMENTO] = @FORMAPAGAMENTO
            WHERE
	            [ID] = @ID";
    protected override string SqlExcluir => @"DELETE FROM [TBDESPESA]
            WHERE
                [ID] = @ID";
    protected override string SqlSelecionarPorId => @"SELECT
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
    protected override string SqlSelecionarTodos => @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [DATAOCORRENCIA],
	            [VALOR],
	            [FORMAPAGAMENTO]
            FROM
	            [TBDESPESA]";
    private static string SqlAdicionarCategoria => @"INSERT INTO [TBDESPESA_TBCATEGORIA]
            (
	            [DESPESA_ID],
	            [CATEGORIA_ID]
            )
            VALUES
            (
	            @DESPESA_ID,
	            @CATEGORIA_ID
            )";
    private static string SqlRemoverCategoria => @"DELETE FROM 
                [TBDESPESA_TBCATEGORIA]
            WHERE
                [DESPESA_ID] = @DESPESA_ID
            AND
                [CATEGORIA_ID] = @CATEGORIA_ID";
    private static string SqlAdicionarCategorias => @"INSERT INTO [TBDESPESA_TBCATEGORIA]
            (
	            [DESPESA_ID],
	            [CATEGORIA_ID]
            )
            VALUES
            (
	            @DESPESA_ID,
	            @CATEGORIA_ID
            )";
    private static string SqlCarregarCategorias => @"SELECT
	            [ID],
	            [TITULO]
            FROM
	            [TBCATEGORIA] AS CAT
	            INNER JOIN [TBDESPESA_TBCATEGORIA] AS DC
            ON
	            CAT.[ID] = DC.[CATEGORIA_ID]
            WHERE
	            DC.[DESPESA_ID] = @DESPESA_ID";
    private static string SqlRemoverCategorias => @"DELETE FROM 
                [TBDESPESA_TBCATEGORIA]
            WHERE
                [DESPESA_ID] = @DESPESA_ID";

    public RepositorioDespesaSQL(IDbConnection conexaoComBanco) : base(conexaoComBanco) { }

    public override void CadastrarRegistro(Despesa novoRegistro)
    {
        base.CadastrarRegistro(novoRegistro);

        AdicionarCategorias(novoRegistro);
    }

    public override bool EditarRegistro(Guid idRegistro, Despesa registroEditado)
    {
        bool registroFoiEditado = base.EditarRegistro(idRegistro, registroEditado);

        RemoverCategorias(idRegistro);
        AdicionarCategorias(registroEditado);

        return registroFoiEditado;
    }

    public override bool ExcluirRegistro(Guid idRegistro)
    {
        RemoverCategorias(idRegistro);

        return base.ExcluirRegistro(idRegistro);
    }

    public override Despesa? SelecionarRegistroPorId(Guid idRegistro)
    {
        Despesa? despesa = base.SelecionarRegistroPorId(idRegistro);

        if (despesa is not null)
            CarregarCategorias(despesa);

        return despesa;
    }

    public override List<Despesa> SelecionarRegistros()
    {
        List<Despesa> despesas = base.SelecionarRegistros();

        foreach (Despesa despesa in despesas)
            CarregarCategorias(despesa);

        return despesas;
    }

    public void AdicionarCategoria(Categoria categoria, Despesa despesa)
    {
        IDbCommand comandoAdicao = conexaoComBanco.CreateCommand();
        comandoAdicao.CommandText = SqlAdicionarCategoria;

        comandoAdicao.AdicionarParametro("DESPESA_ID", despesa.Id);
        comandoAdicao.AdicionarParametro("CATEGORIA_ID", categoria.Id);

        conexaoComBanco.Open();

        comandoAdicao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void RemoverCategoria(Categoria categoria, Despesa despesa)
    {
        IDbCommand comandoExclusao = conexaoComBanco.CreateCommand();
        comandoExclusao.CommandText = SqlRemoverCategoria;

        comandoExclusao.AdicionarParametro("CATEGORIA_ID", categoria.Id);
        comandoExclusao.AdicionarParametro("DESPESA_ID", despesa.Id);

        conexaoComBanco.Open();

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    private void AdicionarCategorias(Despesa despesa)
    {
        conexaoComBanco.Open();

        foreach (Categoria categoria in despesa.Categorias)
        {
            IDbCommand comandoAdicao = conexaoComBanco.CreateCommand();
            comandoAdicao.CommandText = SqlAdicionarCategorias;

            comandoAdicao.AdicionarParametro("DESPESA_ID", despesa.Id);
            comandoAdicao.AdicionarParametro("CATEGORIA_ID", categoria.Id);

            comandoAdicao.ExecuteNonQuery();
        }

        conexaoComBanco.Close();
    }

    private void CarregarCategorias(Despesa despesa)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlCarregarCategorias;

        comandoSelecao.AdicionarParametro("DESPESA_ID", despesa.Id);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            despesa.AderirCategoria(ConverterParaCategoria(leitor));
        }

        conexaoComBanco.Close();
    }

    private void RemoverCategorias(Guid idDespesa)
    {
        IDbCommand comandoExclusao = conexaoComBanco.CreateCommand();
        comandoExclusao.CommandText = SqlRemoverCategorias;

        comandoExclusao.AdicionarParametro("DESPESA_ID", idDespesa);

        conexaoComBanco.Open();

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    private static Categoria ConverterParaCategoria(IDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["Titulo"])!);
    }

    protected override Despesa ConverterParaRegistro(IDataReader leitor)
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

    protected override void ConfigurarParametrosRegistro(Despesa despesa, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", despesa.Id);
        comando.AdicionarParametro("TITULO", despesa.Titulo);
        comando.AdicionarParametro("DESCRICAO", despesa.Descricao);
        comando.AdicionarParametro("DATAOCORRENCIA", despesa.DataOcorrencia);
        comando.AdicionarParametro("VALOR", despesa.Valor);
        comando.AdicionarParametro("FORMAPAGAMENTO", despesa.FormaPagamento);
    }
}
