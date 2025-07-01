using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloContato;

public class RepositorioContatoSQL : IRepositorioContato
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Contato novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        const string sqlCadastrar =
            @"INSERT INTO [TBCONTATO]
            (
                [ID],
                [NOME],
                [EMAIL],
                [TELEFONE],
                [EMPRESA],
                [CARGO]
            )
            VALUES
            (
                @ID,
                @NOME,
                @EMAIL,
                @TELEFONE,
                @EMPRESA,
                @CARGO
            ); SELECT SCOPE_IDENTITY();";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoCadastrar = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosContato(novoRegistro, comandoCadastrar);

        conexaoComBanco.Open();

        comandoCadastrar.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Contato registroEditado)
    {
        const string sqlEditar =
            @"UPDATE [TBCONTATO]	
		    SET
			    [NOME] = @NOME,
			    [EMAIL] = @EMAIL,
			    [TELEFONE] = @TELEFONE,
			    [EMPRESA] = @EMPRESA,
			    [CARGO] = @CARGO
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosContato(registroEditado, comandoEdicao);

        conexaoComBanco.Open();

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        const string sqlExcluir =
            @"DELETE FROM [TBCONTATO]
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null)
    {
        throw new NotImplementedException();
    }

    public bool PossuiCompromissosVinculados(Guid id)
    {
        throw new NotImplementedException();
    }

    public Contato? SelecionarRegistroPorId(Guid idRegistro)
    {
        const string sqlSelecionarPorId =
            @"SELECT 
		        [ID], 
		        [NOME], 
		        [EMAIL],
		        [TELEFONE],
		        [EMPRESA],
		        [CARGO]
	        FROM 
		        [TBCONTATO]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoSelecao = new(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Contato? contato = null;

        if (leitor.Read())
            contato = ConverterParaContato(leitor);

        conexaoComBanco.Close();

        return contato;
    }

    public List<Contato> SelecionarRegistros()
    {
        const string sqlSelecionarTodos =
            @"SELECT
                [ID],
                [NOME],
                [EMAIL],
                [TELEFONE],
                [EMPRESA],
                [CARGO]
            FROM
                [TBCONTATO]";

        SqlConnection conexaoComBanco = new(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Contato> contatos = [];

        while (leitor.Read())
        {
            contatos.Add(ConverterParaContato(leitor));
        }

        conexaoComBanco.Close();

        return contatos;
    }

    private Contato ConverterParaContato(SqlDataReader leitor)
    {
        return new(
            Convert.ToString(leitor["NOME"])!,
            Convert.ToString(leitor["EMAIL"])!,
            Convert.ToString(leitor["TELEFONE"])!,
            Convert.ToString(leitor["CARGO"]),
            Convert.ToString(leitor["EMPRESA"])
            )
        {
            Id = Guid.Parse(leitor["ID"].ToString()!)
        };
    }

    private void ConfigurarParametrosContato(Contato contato, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", contato.Id);
        comando.Parameters.AddWithValue("NOME", contato.Nome);
        comando.Parameters.AddWithValue("EMAIL", contato.Email);
        comando.Parameters.AddWithValue("TELEFONE", contato.Telefone);
        comando.Parameters.AdicionarValorNullavel("EMPRESA", contato.Empresa);
        comando.Parameters.AdicionarValorNullavel("CARGO", contato.Cargo);
    }
}
