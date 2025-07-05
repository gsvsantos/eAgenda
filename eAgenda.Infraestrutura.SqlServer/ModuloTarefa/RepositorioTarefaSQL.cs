using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloTarefa;

public class RepositorioTarefaSQL : IRepositorioTarefa
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Tarefa novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        const string sqlCadastrar =
            @"INSERT INTO [TBTAREFA]
            (
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            )
            VALUES
            (
	            @ID,
	            @TITULO,
	            @DESCRICAO,
	            @PRIORIDADE,
	            @DATACRIACAO,
	            @DATACONCLUSAO,
	            @STATUS
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCadastro = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosTarefa(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Tarefa registroEditado)
    {
        const string sqlEditar =
            @"UPDATE [TBTAREFA]
            SET
	            [TITULO] = @TITULO,
	            [DESCRICAO] = @DESCRICAO,
	            [PRIORIDADE] = @PRIORIDADE,
	            [DATACRIACAO] = @DATACRIACAO,
	            [DATACONCLUSAO] = @DATACONCLUSAO,
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosTarefa(registroEditado, comandoEdicao);

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        const string sqlExcluir =
            @"DELETE FROM [TBTAREFA]
            WHERE
	            [ID] = @ID";

        ExcluirItensTarefa(idRegistro);

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public Tarefa? SelecionarRegistroPorId(Guid idRegistro)
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Tarefa? tarefa = null;

        if (leitor.Read())
            tarefa = ConverterParaTarefa(leitor);

        conexaoComBanco.Close();

        return tarefa;
    }

    public List<Tarefa> SelecionarRegistros()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public void AtualizarStatusRegistros()
    {
        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        foreach (Tarefa tarefa in SelecionarRegistros())
        {
            tarefa.AtualizarStatus();
            AtualizarStatusTarefa(tarefa);
        }

        conexaoComBanco.Close();
    }

    public void AdicionarItem(ItemTarefa item)
    {
        const string sqlAdicionarItem =
            @"INSERT INTO [TBITEMTAREFA]
            (
	            [ID],
	            [TITULO],
	            [STATUS],
	            [TAREFA_ID]
            )
            VALUES
            (
	            @ID,
	            @TITULO,
	            @STATUS,
	            @TAREFA_ID
            )";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoAdicao = new(sqlAdicionarItem, conexaoComBanco);

        ConfigurarParametrosItemTarefa(item, comandoAdicao);

        comandoAdicao.ExecuteScalar();

        conexaoComBanco.Close();
    }

    public void RemoverItem(ItemTarefa item)
    {
        const string sqlRemoverItem =
            @"DELETE FROM [TBITEMTAREFA]
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoRemocao = new(sqlRemoverItem, conexaoComBanco);

        comandoRemocao.Parameters.AddWithValue("ID", item.Id);

        comandoRemocao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ConcluirItem(ItemTarefa item)
    {
        item.Concluir();

        const string sqlConcluirItem =
            @"UPDATE [TBITEMTAREFA]
            SET
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoConclusao = new(sqlConcluirItem, conexaoComBanco);

        comandoConclusao.Parameters.AddWithValue("STATUS", item.Status);
        comandoConclusao.Parameters.AddWithValue("ID", item.Id);

        comandoConclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ReabrirItem(ItemTarefa item)
    {
        item.Reabrir();

        const string sqlReabrirItem =
            @"UPDATE [TBITEMTAREFA]
            SET
                [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoReabertura = new(sqlReabrirItem, conexaoComBanco);

        comandoReabertura.Parameters.AddWithValue("STATUS", item.Status);
        comandoReabertura.Parameters.AddWithValue("ID", item.Id);

        comandoReabertura.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void CancelarItensTarefa(Tarefa tarefa)
    {
        tarefa.Cancelar();

        const string sqlCancelarItens =
            @"UPDATE [TBITEMTAREFA]
            SET
                [STATUS] = @STATUS
            WHERE 
                [TAREFA_ID] = @TAREFA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoCancelamento = new(sqlCancelarItens, conexaoComBanco);

        comandoCancelamento.Parameters.AddWithValue("STATUS", StatusItemTarefa.Cancelado);
        comandoCancelamento.Parameters.AddWithValue("TAREFA_ID", tarefa.Id);

        comandoCancelamento.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public void ConcluirItensTarefa(Tarefa tarefa)
    {
        tarefa.Concluir();

        const string sqlCancelarItens =
            @"UPDATE [TBITEMTAREFA]
            SET
                [STATUS] = @STATUS
            WHERE 
                [TAREFA_ID] = @TAREFA_ID";

        SqlConnection conexaoComBanco = new(connectionString);
        conexaoComBanco.Open();

        SqlCommand comandoConclusao = new(sqlCancelarItens, conexaoComBanco);

        comandoConclusao.Parameters.AddWithValue("STATUS", StatusItemTarefa.Concluido);
        comandoConclusao.Parameters.AddWithValue("TAREFA_ID", tarefa.Id);

        comandoConclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public void ExcluirItensTarefa(Guid idTarefa)
    {
        const string sqlExcluirItensTarefa =
            @"DELETE FROM [TBITEMTAREFA]
		    WHERE
			    [TAREFA_ID] = @TAREFA_ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoExclusao = new SqlCommand(sqlExcluirItensTarefa, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("TAREFA_ID", idTarefa);

        conexaoComBanco.Open();

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ReabrirItensTarefa(Tarefa tarefa)
    {
        tarefa.Reabrir();

        const string sqlCancelarItens =
            @"UPDATE [TBITEMTAREFA]
            SET
                [STATUS] = @STATUS
            WHERE 
                [TAREFA_ID] = @TAREFA_ID";

        SqlConnection conexaoComBanco = new(connectionString);
        conexaoComBanco.Open();

        SqlCommand comandoReabertura = new(sqlCancelarItens, conexaoComBanco);

        comandoReabertura.Parameters.AddWithValue("STATUS", StatusItemTarefa.EmAndamento);
        comandoReabertura.Parameters.AddWithValue("TAREFA_ID", tarefa.Id);

        comandoReabertura.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem)
    {
        const string sqlSelecionarItem =
            @"SELECT
	            [ID],
	            [TITULO],
	            [STATUS],
	            [TAREFA_ID]
            FROM
	            [TBITEMTAREFA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarItem, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idItem);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        ItemTarefa? item = null;

        if (leitor.Read())
            item = ConverterParaItemTarefa(leitor, tarefa);

        conexaoComBanco.Close();

        return item;
    }

    public List<Tarefa> SelecionarTarefasCanceladas()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [STATUS] = @STATUS";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("STATUS", StatusTarefa.Cancelada);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasConcluidas()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [STATUS] = @STATUS";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("STATUS", StatusTarefa.Concluida);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasEmAndamento()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [STATUS] = @STATUS";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("STATUS", StatusTarefa.EmAndamento);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPendentes()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [STATUS] = @STATUS";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("STATUS", StatusTarefa.Pendente);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPrioridadeAlta()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [PRIORIDADE] = @PRIORIDADE";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("PRIORIDADE", NivelPrioridade.Alta);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPrioridadeBaixa()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [PRIORIDADE] = @PRIORIDADE";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("PRIORIDADE", NivelPrioridade.Baixa);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPrioridadeMedia()
    {
        const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]
            WHERE
                [PRIORIDADE] = @PRIORIDADE";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("PRIORIDADE", NivelPrioridade.Media);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaTarefa(leitor));
        }

        conexaoComBanco.Close();

        return tarefas;
    }

    private void AtualizarStatusTarefa(Tarefa tarefa)
    {
        const string sqlAtualizarTarefa =
            @"UPDATE [TBTAREFA]
            SET
	            [DATACONCLUSAO] = @DATACONCLUSAO,
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoAtualizacao = new(sqlAtualizarTarefa, conexaoComBanco);

        comandoAtualizacao.Parameters.AddWithValue("ID", tarefa.Id);
        comandoAtualizacao.Parameters.AdicionarValorNullavel("DATACONCLUSAO", tarefa.DataConclusao);
        comandoAtualizacao.Parameters.AddWithValue("STATUS", tarefa.Status);

        comandoAtualizacao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    private Tarefa ConverterParaTarefa(SqlDataReader leitor)

    {
        DateTime? dataConclusao = null;

        if (!leitor["DATACONCLUSAO"].Equals(DBNull.Value))
            dataConclusao = Convert.ToDateTime(leitor["DATACONCLUSAO"]);

        Tarefa tarefa = new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["TITULO"])!,
            Convert.ToString(leitor["DESCRICAO"])!,
            (NivelPrioridade)Convert.ToInt64(leitor["PRIORIDADE"]),
            Convert.ToDateTime(leitor["DATACRIACAO"]),
            dataConclusao,
            (StatusTarefa)Convert.ToInt64(leitor["STATUS"])
        );

        CarregarItensTarefa(tarefa);

        return tarefa;
    }

    private void CarregarItensTarefa(Tarefa tarefa)
    {
        const string sqlSelecionarItensTarefa =
            @"SELECT 
	            [ID],
	            [TITULO],
	            [STATUS],
	            [TAREFA_ID]
            FROM 
	            [TBITEMTAREFA]
            WHERE
	            [TAREFA_ID] = @TAREFA_ID";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarItensTarefa, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("TAREFA_ID", tarefa.Id);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            tarefa.AdicionarItem(ConverterParaItemTarefa(leitor, tarefa));
        }

        conexaoComBanco.Close();
    }

    private ItemTarefa ConverterParaItemTarefa(SqlDataReader leitor, Tarefa tarefa)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["TITULO"])!,
            (StatusItemTarefa)Convert.ToInt64(leitor["STATUS"]),
            tarefa
        );
    }

    private void ConfigurarParametrosTarefa(Tarefa tarefa, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", tarefa.Id);
        comando.Parameters.AddWithValue("TITULO", tarefa.Titulo);
        comando.Parameters.AddWithValue("DESCRICAO", tarefa.Descricao);
        comando.Parameters.AddWithValue("PRIORIDADE", tarefa.Prioridade);
        comando.Parameters.AddWithValue("DATACRIACAO", tarefa.DataCriacao);
        comando.Parameters.AdicionarValorNullavel("DATACONCLUSAO", tarefa.DataConclusao);
        comando.Parameters.AddWithValue("STATUS", tarefa.Status);
    }

    private void ConfigurarParametrosItemTarefa(ItemTarefa item, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", item.Id);
        comando.Parameters.AddWithValue("TITULO", item.Titulo);
        comando.Parameters.AddWithValue("STATUS", item.Status);
        comando.Parameters.AddWithValue("TAREFA_ID", item.Tarefa.Id);
    }
}
