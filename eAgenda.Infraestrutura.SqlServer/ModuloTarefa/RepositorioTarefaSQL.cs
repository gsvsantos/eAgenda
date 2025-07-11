using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.ModuloTarefa;

public class RepositorioTarefaSQL : RepositorioBaseSQL<Tarefa>, IRepositorioTarefa
{
    protected override string SqlCadastrar => @"INSERT INTO [TBTAREFA]
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
    protected override string SqlEditar => @"UPDATE [TBTAREFA]
            SET
	            [TITULO] = @TITULO,
	            [DESCRICAO] = @DESCRICAO,
	            [PRIORIDADE] = @PRIORIDADE,
	            [DATACRIACAO] = @DATACRIACAO,
	            [DATACONCLUSAO] = @DATACONCLUSAO,
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";
    protected override string SqlExcluir => @"DELETE FROM [TBTAREFA]
            WHERE
	            [ID] = @ID";
    protected override string SqlSelecionarPorId => @"SELECT
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
    protected override string SqlSelecionarTodos => @"SELECT
	            [ID],
	            [TITULO],
	            [DESCRICAO],
	            [PRIORIDADE],
	            [DATACRIACAO],
	            [DATACONCLUSAO],
	            [STATUS]
            FROM
	            [TBTAREFA]";
    private static string SqlAdicionarItem => @"INSERT INTO [TBITEMTAREFA]
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
    private static string SqlRemoverItem => @"DELETE FROM [TBITEMTAREFA]
            WHERE
	            [ID] = @ID";
    private static string SqlAtualizarStatusItem => @"UPDATE [TBITEMTAREFA]
            SET
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";
    private static string SqlAtualizarStatusItens => @"UPDATE [TBITEMTAREFA]
            SET
                [STATUS] = @STATUS
            WHERE 
                [TAREFA_ID] = @TAREFA_ID";
    private static string SqlExcluirItensTarefa => @"DELETE FROM [TBITEMTAREFA]
		    WHERE
			    [TAREFA_ID] = @TAREFA_ID";
    private static string SqlSelecionarItem => @"SELECT
	            [ID],
	            [TITULO],
	            [STATUS],
	            [TAREFA_ID]
            FROM
	            [TBITEMTAREFA]
            WHERE
                [ID] = @ID";
    private static string SqlSelecionarPorStatus => @"SELECT
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
    private static string SqlSelecionarPorPrioridade => @"SELECT
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
    private static string SqlAtualizarStatusTarefa => @"UPDATE [TBTAREFA]
            SET
	            [DATACONCLUSAO] = @DATACONCLUSAO,
	            [STATUS] = @STATUS
            WHERE
	            [ID] = @ID";
    private static string SqlSelecionarItensTarefa => @"SELECT 
	            [ID],
	            [TITULO],
	            [STATUS],
	            [TAREFA_ID]
            FROM 
	            [TBITEMTAREFA]
            WHERE
	            [TAREFA_ID] = @TAREFA_ID";

    public RepositorioTarefaSQL(IDbConnection conexaoComBanco) : base(conexaoComBanco) { }

    public void AtualizarStatusRegistros()
    {
        foreach (Tarefa tarefa in SelecionarRegistros())
        {
            tarefa.AtualizarStatus();
            AtualizarStatusTarefa(tarefa);
        }
    }

    public void AdicionarItem(ItemTarefa item)
    {
        IDbCommand comandoAdicao = conexaoComBanco.CreateCommand();
        comandoAdicao.CommandText = SqlAdicionarItem;

        ConfigurarParametrosItemTarefa(item, comandoAdicao);

        conexaoComBanco.Open();

        comandoAdicao.ExecuteScalar();

        conexaoComBanco.Close();
    }

    public void RemoverItem(ItemTarefa item)
    {
        IDbCommand comandoRemocao = conexaoComBanco.CreateCommand();
        comandoRemocao.CommandText = SqlRemoverItem;

        comandoRemocao.AdicionarParametro("ID", item.Id);

        conexaoComBanco.Open();

        comandoRemocao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ConcluirItem(ItemTarefa item)
    {
        item.Concluir();

        IDbCommand comandoConclusao = conexaoComBanco.CreateCommand();
        comandoConclusao.CommandText = SqlAtualizarStatusItem;

        comandoConclusao.AdicionarParametro("STATUS", item.Status);
        comandoConclusao.AdicionarParametro("ID", item.Id);

        conexaoComBanco.Open();

        comandoConclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ReabrirItem(ItemTarefa item)
    {
        item.Reabrir();

        IDbCommand comandoReabertura = conexaoComBanco.CreateCommand();
        comandoReabertura.CommandText = SqlAtualizarStatusItem;

        comandoReabertura.AdicionarParametro("STATUS", item.Status);
        comandoReabertura.AdicionarParametro("ID", item.Id);

        conexaoComBanco.Open();

        comandoReabertura.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void CancelarItensTarefa(Tarefa tarefa)
    {
        tarefa.Cancelar();

        IDbCommand comandoCancelamento = conexaoComBanco.CreateCommand();
        comandoCancelamento.CommandText = SqlAtualizarStatusItens;

        comandoCancelamento.AdicionarParametro("STATUS", StatusItemTarefa.Cancelado);
        comandoCancelamento.AdicionarParametro("TAREFA_ID", tarefa.Id);

        conexaoComBanco.Open();

        comandoCancelamento.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public void ConcluirItensTarefa(Tarefa tarefa)
    {
        tarefa.Concluir();

        IDbCommand comandoConclusao = conexaoComBanco.CreateCommand();
        comandoConclusao.CommandText = SqlAtualizarStatusItens;

        comandoConclusao.AdicionarParametro("STATUS", StatusItemTarefa.Concluido);
        comandoConclusao.AdicionarParametro("TAREFA_ID", tarefa.Id);

        conexaoComBanco.Open();

        comandoConclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public void ExcluirItensTarefa(Guid idTarefa)
    {
        IDbCommand comandoExclusao = conexaoComBanco.CreateCommand();
        comandoExclusao.CommandText = SqlExcluirItensTarefa;

        comandoExclusao.AdicionarParametro("TAREFA_ID", idTarefa);

        conexaoComBanco.Open();

        comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public void ReabrirItensTarefa(Tarefa tarefa)
    {
        tarefa.Reabrir();

        IDbCommand comandoReabertura = conexaoComBanco.CreateCommand();
        comandoReabertura.CommandText = SqlAtualizarStatusItens;

        comandoReabertura.AdicionarParametro("STATUS", StatusItemTarefa.EmAndamento);
        comandoReabertura.AdicionarParametro("TAREFA_ID", tarefa.Id);

        conexaoComBanco.Open();

        comandoReabertura.ExecuteNonQuery();

        conexaoComBanco.Close();

        AtualizarStatusTarefa(tarefa);
    }

    public ItemTarefa? SelecionarItem(Tarefa tarefa, Guid idItem)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarItem;

        comandoSelecao.AdicionarParametro("ID", idItem);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        ItemTarefa? item = null;

        if (leitor.Read())
            item = ConverterParaItemTarefa(leitor, tarefa);

        conexaoComBanco.Close();

        return item;
    }

    public override Tarefa? SelecionarRegistroPorId(Guid idRegistro)
    {
        Tarefa? tarefa = base.SelecionarRegistroPorId(idRegistro);

        if (tarefa is not null)
            CarregarItensTarefa(tarefa);

        return tarefa;
    }

    public override List<Tarefa> SelecionarRegistros()
    {
        List<Tarefa> tarefas = base.SelecionarRegistros();

        foreach (Tarefa tarefa in tarefas)
            CarregarItensTarefa(tarefa);

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPorStatus(string? status)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarPorStatus;

        StatusTarefa? statusAtual = status switch
        {
            "Pendente" => StatusTarefa.Pendente,
            "EmAndamento" => StatusTarefa.EmAndamento,
            "Concluida" => StatusTarefa.Concluida,
            "Cancelada" => StatusTarefa.Cancelada,
            _ => null
        };

        comandoSelecao.AdicionarParametro("STATUS", statusAtual!.Value);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaRegistro(leitor));
        }

        conexaoComBanco.Close();

        foreach (Tarefa tarefa in tarefas)
            CarregarItensTarefa(tarefa);

        return tarefas;
    }

    public List<Tarefa> SelecionarTarefasPorPrioridade(string? prioridade)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarPorPrioridade;

        NivelPrioridade? prioridadeAtual = prioridade switch
        {
            "Baixa" => NivelPrioridade.Baixa,
            "Media" => NivelPrioridade.Media,
            "Alta" => NivelPrioridade.Alta,
            _ => null
        };

        comandoSelecao.AdicionarParametro("PRIORIDADE", prioridadeAtual!.Value);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        List<Tarefa> tarefas = [];

        while (leitor.Read())
        {
            tarefas.Add(ConverterParaRegistro(leitor));
        }

        conexaoComBanco.Close();

        foreach (Tarefa tarefa in tarefas)
            CarregarItensTarefa(tarefa);

        return tarefas;
    }

    private void AtualizarStatusTarefa(Tarefa tarefa)
    {
        IDbCommand comandoAtualizacao = conexaoComBanco.CreateCommand();
        comandoAtualizacao.CommandText = SqlAtualizarStatusTarefa;

        comandoAtualizacao.AdicionarParametro("ID", tarefa.Id);
        comandoAtualizacao.AdicionarParametro("DATACONCLUSAO", tarefa.DataConclusao!);
        comandoAtualizacao.AdicionarParametro("STATUS", tarefa.Status);

        conexaoComBanco.Open();

        comandoAtualizacao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    private void CarregarItensTarefa(Tarefa? tarefa)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarItensTarefa;

        comandoSelecao.AdicionarParametro("TAREFA_ID", tarefa!.Id);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            tarefa.AdicionarItem(ConverterParaItemTarefa(leitor, tarefa));
        }

        conexaoComBanco.Close();
    }

    private static ItemTarefa ConverterParaItemTarefa(IDataReader leitor, Tarefa tarefa)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["TITULO"])!,
            (StatusItemTarefa)Convert.ToInt64(leitor["STATUS"]),
            tarefa
        )
        {
            Tarefa = tarefa
        };
    }

    private static void ConfigurarParametrosItemTarefa(ItemTarefa item, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", item.Id);
        comando.AdicionarParametro("TITULO", item.Titulo);
        comando.AdicionarParametro("STATUS", item.Status);
        comando.AdicionarParametro("TAREFA_ID", item.Tarefa.Id);
    }

    protected override Tarefa ConverterParaRegistro(IDataReader leitor)
    {
        DateTime? dataConclusao = null;

        if (!leitor["DATACONCLUSAO"].Equals(DBNull.Value))
            dataConclusao = Convert.ToDateTime(leitor["DATACONCLUSAO"]);

        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["TITULO"])!,
            Convert.ToString(leitor["DESCRICAO"])!,
            (NivelPrioridade)Convert.ToInt64(leitor["PRIORIDADE"]),
            Convert.ToDateTime(leitor["DATACRIACAO"]),
            dataConclusao,
            (StatusTarefa)Convert.ToInt64(leitor["STATUS"])
        );
    }

    protected override void ConfigurarParametrosRegistro(Tarefa tarefa, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", tarefa.Id);
        comando.AdicionarParametro("TITULO", tarefa.Titulo);
        comando.AdicionarParametro("DESCRICAO", tarefa.Descricao);
        comando.AdicionarParametro("PRIORIDADE", tarefa.Prioridade);
        comando.AdicionarParametro("DATACRIACAO", tarefa.DataCriacao);
        comando.AdicionarParametro("DATACONCLUSAO", tarefa.DataConclusao!);
        comando.AdicionarParametro("STATUS", tarefa.Status);
    }
}
