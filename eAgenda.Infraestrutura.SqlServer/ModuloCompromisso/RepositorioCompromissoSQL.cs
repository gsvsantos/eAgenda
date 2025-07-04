using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infraestrutura.SQLServer.ModuloCompromisso;

public class RepositorioCompromissoSQL : IRepositorioCompromisso
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=eAgendaDb;Integrated Security=True";

    public void CadastrarRegistro(Compromisso novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();

        const string sqlCadastrar =
            @"INSERT INTO [TBCOMPROMISSO]
            (
	            [ID],
	            [ASSUNTO],
	            [DATA],
	            [HORAINICIO],
	            [HORATERMINO],
	            [TIPO],
	            [LOCAL],
	            [LINK],
	            [CONTATO_ID]
            )
            VALUES
            (
                @ID,
                @ASSUNTO,
                @DATA,
                @HORAINICIO,
                @HORATERMINO,
                @TIPO,
                @LOCAL,
                @LINK,
                @CONTATO_ID
            );";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoCadastro = new(sqlCadastrar, conexaoComBanco);

        ConfigurarParametrosCompromisso(novoRegistro, comandoCadastro);

        conexaoComBanco.Open();

        comandoCadastro.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, Compromisso registroEditado)
    {
        const string sqlEditar =
            @"UPDATE [TBCOMPROMISSO]
            SET 
                [ASSUNTO] = @ASSUNTO,
                [DATA] = @DATA, 
                [HORAINICIO] = @HORAINICIO, 
                [HORATERMINO] = @HORATERMINO,
                [TIPO] = @TIPO,
                [LOCAL] = @LOCAL, 
                [LINK] = @LINK,
                [CONTATO_ID] = @CONTATO_ID
            WHERE 
                [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametrosCompromisso(registroEditado, comandoEdicao);

        conexaoComBanco.Open();

        int linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        const string sqlExcluir =
            @"DELETE FROM [TBCOMPROMISSO]
	        WHERE [ID] = @ID";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        int linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas >= 1;
    }

    public Compromisso? SelecionarRegistroPorId(Guid idRegistro)
    {
        const string sqlSelecionarPorId =
            @"SELECT
	            CP.[ID],
	            CP.[ASSUNTO],
	            CP.[DATA],
	            CP.[HORAINICIO],
	            CP.[HORATERMINO],
	            CP.[TIPO],
	            CP.[LOCAL],
	            CP.[LINK],
	            CP.[CONTATO_ID],
	            CT.[NOME],
	            CT.[EMAIL],
	            CT.[TELEFONE],
	            CT.[EMPRESA],
	            CT.[CARGO]
            FROM
	            [TBCOMPROMISSO] AS CP LEFT JOIN
	            [TBCONTATO] AS CT
            ON
	            CT.ID = CP.CONTATO_ID
            WHERE
	            CP.ID = @ID;";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoSelecao = new(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Compromisso? compromisso = null;

        if (leitor.Read())
            compromisso = ConverterParaCompromisso(leitor);

        conexaoComBanco.Close();

        return compromisso;
    }

    public List<Compromisso> SelecionarRegistros()
    {
        const string sqlSelecionarTodos =
            @"SELECT
                CP.[ID],
                CP.[ASSUNTO],
                CP.[DATA],
                CP.[HORAINICIO],
                CP.[HORATERMINO],
                CP.[TIPO],
                CP.[LOCAL],
                CP.[LINK],
                CP.[CONTATO_ID],
                CT.[NOME],
                CT.[EMAIL],
                CT.[TELEFONE],
                CT.[EMPRESA],
                CT.[CARGO]
            from
	            [TBCOMPROMISSO] AS CP LEFT JOIN
	            [TBCONTATO] AS CT
            on
	            CT.ID = CP.CONTATO_ID;";

        SqlConnection conexaoComBanco = new(connectionString);

        SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        List<Compromisso> compromissos = [];

        while (leitor.Read())
        {
            Compromisso compromisso = ConverterParaCompromisso(leitor);

            compromissos.Add(compromisso);
        }

        conexaoComBanco.Close();

        return compromissos;
    }

    private Compromisso ConverterParaCompromisso(SqlDataReader leitor)
    {
        Contato? contato = null;

        if (!leitor["CONTATO_ID"].Equals(DBNull.Value))
            contato = ConverterParaContato(leitor);

        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["ASSUNTO"])!,
            Convert.ToDateTime(leitor["DATA"]),
            TimeSpan.FromTicks(Convert.ToInt64(leitor["HORAINICIO"])),
            TimeSpan.FromTicks(Convert.ToInt64(leitor["HORATERMINO"])),
            (TipoCompromisso)Convert.ToInt32(leitor["TIPO"]),
            Convert.ToString(leitor["LOCAL"])!,
            Convert.ToString(leitor["LINK"])!,
            contato
            );
    }

    private Contato ConverterParaContato(SqlDataReader leitor)
    {
        return new(
            Guid.Parse(leitor["ID"].ToString()!),
            Convert.ToString(leitor["NOME"])!,
            Convert.ToString(leitor["EMAIL"])!,
            Convert.ToString(leitor["TELEFONE"])!,
            Convert.ToString(leitor["CARGO"]),
            Convert.ToString(leitor["EMPRESA"])
            );
    }

    private void ConfigurarParametrosCompromisso(Compromisso compromisso, SqlCommand comando)
    {
        comando.Parameters.AddWithValue("ID", compromisso.Id);
        comando.Parameters.AddWithValue("ASSUNTO", compromisso.Assunto);
        comando.Parameters.AddWithValue("DATA", compromisso.DataOcorrencia);
        comando.Parameters.AddWithValue("HORAINICIO", compromisso.HoraInicio.Ticks);
        comando.Parameters.AddWithValue("HORATERMINO", compromisso.HoraTermino.Ticks);
        comando.Parameters.AddWithValue("TIPO", (int)compromisso.TipoCompromisso);
        comando.Parameters.AdicionarValorNullavel("LOCAL", compromisso.Local);
        comando.Parameters.AdicionarValorNullavel("LINK", compromisso.Link);
        comando.Parameters.AdicionarValorNullavel("CONTATO_ID", compromisso.Contato?.Id);
    }

    public bool TemConflito(Compromisso compromisso)
    {
        const string sqlVerificacao =
            @"SELECT COUNT(*) 
            FROM TBCompromisso 
            WHERE 
                ID <> @ID AND
                DATA = @DATA AND
                (
                    (@HoraInicio >= HoraInicio AND @HoraInicio < HoraTermino) OR
                    (@HoraTermino > HoraInicio AND @HoraTermino <= HoraTermino) OR
                    (@HoraInicio <= HoraInicio AND @HoraTermino >= HoraTermino)
                );";

        SqlConnection conexaoComBanco = new(connectionString);
        SqlCommand comandoSelecao = new(sqlVerificacao, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", compromisso.Id);
        comandoSelecao.Parameters.AddWithValue("DATA", compromisso.DataOcorrencia);
        comandoSelecao.Parameters.AddWithValue("HoraInicio", compromisso.HoraInicio.Ticks);
        comandoSelecao.Parameters.AddWithValue("HoraTermino", compromisso.HoraTermino.Ticks);

        conexaoComBanco.Open();

        int quantidadeConflitos = Convert.ToInt32(comandoSelecao.ExecuteScalar());

        conexaoComBanco.Close();

        return quantidadeConflitos > 0;
    }
}
