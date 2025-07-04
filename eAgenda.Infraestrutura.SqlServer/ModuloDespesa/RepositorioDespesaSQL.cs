﻿using eAgenda.Dominio.ModuloCategoria;
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

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCadastro = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosDespesa(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();

        AdicionarCategorias(novoRegistro);
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

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        RemoverCategorias(idRegistro);

        AdicionarCategorias(registroEditado);

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

        if (leitor.Read())
            despesa = ConverterParaDespesa(leitor);

        if (despesa is not null)
            CarregarCategorias(despesa);

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

        foreach (Despesa despesa in despesas)
            CarregarCategorias(despesa);

        return despesas;
    }

    public void AdicionarCategoria(Categoria categoria, Despesa despesa)
    {
        const string sqlAdicionarCategoria =
            @"INSERT INTO [TBDESPESA_TBCATEGORIA]
            (
	            [DESPESA_ID],
	            [CATEGORIA_ID]
            )
            VALUES
            (
	            @DESPESA_ID,
	            @CATEGORIA_ID
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoAdicao = new(sqlAdicionarCategoria, conexaoComBanco);

        comandoAdicao.Parameters.AddWithValue("DESPESA_ID", despesa.Id);
        comandoAdicao.Parameters.AddWithValue("CATEGORIA_ID", categoria.Id);

        comandoAdicao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void RemoverCategoria(Categoria categoria, Despesa despesa)
    {
        const string sqlRemoverCategoria =
            @"DELETE FROM 
                [TBDESPESA_TBCATEGORIA]
            WHERE
                [DESPESA_ID] = @DESPESA_ID
            AND
                [CATEGORIA_ID] = @CATEGORIA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoExclusao = new(sqlRemoverCategoria, conexaoComBanco);

        conexaoComBanco.Open();

        comandoExclusao.Parameters.AddWithValue("CATEGORIA_ID", categoria.Id);
        comandoExclusao.Parameters.AddWithValue("DESPESA_ID", despesa.Id);

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
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

    private Categoria ConverterParaCategoria(SqlDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["Titulo"])!);
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

    private void AdicionarCategorias(Despesa despesa)
    {
        const string sqlAdicionarCategorias =
            @"INSERT INTO [TBDESPESA_TBCATEGORIA]
            (
	            [DESPESA_ID],
	            [CATEGORIA_ID]
            )
            VALUES
            (
	            @DESPESA_ID,
	            @CATEGORIA_ID
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        foreach (Categoria categoria in despesa.Categorias)
        {
            SqlCommand comandoAdicao = new(sqlAdicionarCategorias, conexaoComBanco);

            comandoAdicao.Parameters.AddWithValue("DESPESA_ID", despesa.Id);
            comandoAdicao.Parameters.AddWithValue("CATEGORIA_ID", categoria.Id);

            comandoAdicao.ExecuteNonQuery();
        }

        conexaoComBanco.Close();
    }

    private void CarregarCategorias(Despesa despesa)
    {
        const string sqlCarregarCategorias =
            @"SELECT
	            [ID],
	            [TITULO]
            FROM
	            [TBCATEGORIA] AS CAT
	            INNER JOIN [TBDESPESA_TBCATEGORIA] AS DC
            ON
	            CAT.[ID] = DC.[CATEGORIA_ID]
            WHERE
	            DC.[DESPESA_ID] = @DESPESA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlCarregarCategorias, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("DESPESA_ID", despesa.Id);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            Categoria categoria = ConverterParaCategoria(leitor);

            despesa.AderirCategoria(categoria);
        }

        conexaoComBanco.Close();
    }

    private void RemoverCategorias(Guid idDespesa)
    {
        const string sqlRemoverCategorias =
            @"DELETE FROM 
                [TBDESPESA_TBCATEGORIA]
            WHERE
                [DESPESA_ID] = @DESPESA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoExclusao = new(sqlRemoverCategorias, conexaoComBanco);

        conexaoComBanco.Open();

        comandoExclusao.Parameters.AddWithValue("DESPESA_ID", idDespesa);

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }
}
