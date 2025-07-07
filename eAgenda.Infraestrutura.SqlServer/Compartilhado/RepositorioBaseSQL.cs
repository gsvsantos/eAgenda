using eAgenda.Dominio.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.Compartilhado;

public abstract class RepositorioBaseSQL<T> where T : EntidadeBase<T>
{
    protected readonly IDbConnection conexaoComBanco;
    protected abstract string SqlCadastrar { get; }
    protected abstract string SqlEditar { get; }
    protected abstract string SqlExcluir { get; }
    protected abstract string SqlSelecionarPorId { get; }
    protected abstract string SqlSelecionarTodos { get; }

    protected RepositorioBaseSQL(IDbConnection conexaoComBanco)
    {
        this.conexaoComBanco = conexaoComBanco;
    }

    public virtual void CadastrarRegistro(T novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        conexaoComBanco.Open();

        IDbCommand comandoCadastro = conexaoComBanco.CreateCommand();
        comandoCadastro.CommandText = SqlCadastrar;

        ConfigurarParametrosRegistro(novoRegistro, comandoCadastro);

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public virtual bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        conexaoComBanco.Open();

        IDbCommand comandoEdicao = conexaoComBanco.CreateCommand();
        comandoEdicao.CommandText = SqlEditar;

        registroEditado.Id = idRegistro;

        ConfigurarParametrosRegistro(registroEditado, comandoEdicao);

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public virtual bool ExcluirRegistro(Guid idRegistro)
    {
        conexaoComBanco.Open();

        IDbCommand comandoExclusao = conexaoComBanco.CreateCommand();
        comandoExclusao.CommandText = SqlExcluir;

        comandoExclusao.AdicionarParametro("ID", idRegistro);

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public virtual T? SelecionarRegistroPorId(Guid idRegistro)
    {
        conexaoComBanco.Open();

        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarPorId;

        comandoSelecao.AdicionarParametro("ID", idRegistro);

        IDataReader leitor = comandoSelecao.ExecuteReader();

        T? T = null;

        if (leitor.Read())
            T = ConverterParaRegistro(leitor);

        conexaoComBanco.Close();

        return T;
    }

    public virtual List<T> SelecionarRegistros()
    {
        conexaoComBanco.Open();

        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarTodos;

        IDataReader leitor = comandoSelecao.ExecuteReader();

        List<T> Ts = [];

        while (leitor.Read())
        {
            Ts.Add(ConverterParaRegistro(leitor));
        }

        conexaoComBanco.Close();

        return Ts;
    }

    protected abstract T ConverterParaRegistro(IDataReader leitor);

    protected abstract void ConfigurarParametrosRegistro(T novoRegistro, IDbCommand comandoCadastro);
}
