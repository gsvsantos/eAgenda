using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.ModuloCompromisso;

public class RepositorioCompromissoSQL : RepositorioBaseSQL<Compromisso>, IRepositorioCompromisso
{
    protected override string SqlCadastrar => @"INSERT INTO [TBCOMPROMISSO]
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
    protected override string SqlEditar => @"UPDATE [TBCOMPROMISSO]
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
                [ID] = @ID;";
    protected override string SqlExcluir => @"DELETE FROM [TBCOMPROMISSO]
	        WHERE [ID] = @ID;";
    protected override string SqlSelecionarPorId => @"SELECT
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
    protected override string SqlSelecionarTodos => @"SELECT
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
    private static string SqlSelecionarCompromissos => @"SELECT
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
	            [TBCOMPROMISSO] AS CP INNER JOIN
	            [TBCONTATO] AS CT
            ON
	            CT.ID = CP.CONTATO_ID
            WHERE
	            CT.ID = @ID;";
    private static string SqlVerificarConflito => @"SELECT COUNT(*) 
            FROM [TBCOMPROMISSO] 
            WHERE 
                ID <> @ID AND
                DATA = @DATA AND
                (
                    (@HORAINICIO >= [HORAINICIO] AND @HORAINICIO < [HORATERMINO]) OR
                    (@HORATERMINO > [HORAINICIO] AND @HORATERMINO <= [HORATERMINO]) OR
                    (@HORAINICIO <= [HORAINICIO] AND @HORATERMINO >= [HORATERMINO])
                );";

    public RepositorioCompromissoSQL(IDbConnection conexaoComBanco) : base(conexaoComBanco) { }

    public List<Compromisso> SelecionarCompromissosContato(Guid idContato)
    {
        IDbCommand comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = SqlSelecionarCompromissos;

        comandoSelecao.AdicionarParametro("ID", idContato);

        conexaoComBanco.Open();

        IDataReader leitor = comandoSelecao.ExecuteReader();

        List<Compromisso> compromissos = [];

        while (leitor.Read())
        {
            compromissos.Add(ConverterParaRegistro(leitor));
        }

        conexaoComBanco.Close();

        return compromissos;
    }

    public bool TemConflito(Compromisso compromisso)
    {
        IDbCommand comandoVerificacao = conexaoComBanco.CreateCommand();
        comandoVerificacao.CommandText = SqlVerificarConflito;

        comandoVerificacao.AdicionarParametro("ID", compromisso.Id);
        comandoVerificacao.AdicionarParametro("DATA", compromisso.DataOcorrencia);
        comandoVerificacao.AdicionarParametro("HORAINICIO", compromisso.HoraInicio.Ticks);
        comandoVerificacao.AdicionarParametro("HORATERMINO", compromisso.HoraTermino.Ticks);

        conexaoComBanco.Open();

        int quantidadeConflitos = Convert.ToInt32(comandoVerificacao.ExecuteScalar());

        conexaoComBanco.Close();

        return quantidadeConflitos > 0;
    }

    private static Contato ConverterParaContato(IDataReader leitor)
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

    protected override Compromisso ConverterParaRegistro(IDataReader leitor)
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
            (TipoCompromisso)Convert.ToInt64(leitor["TIPO"]),
            Convert.ToString(leitor["LOCAL"])!,
            Convert.ToString(leitor["LINK"])!,
            contato
            );
    }

    protected override void ConfigurarParametrosRegistro(Compromisso compromisso, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", compromisso.Id);
        comando.AdicionarParametro("ASSUNTO", compromisso.Assunto);
        comando.AdicionarParametro("DATA", compromisso.DataOcorrencia);
        comando.AdicionarParametro("HORAINICIO", compromisso.HoraInicio.Ticks);
        comando.AdicionarParametro("HORATERMINO", compromisso.HoraTermino.Ticks);
        comando.AdicionarParametro("TIPO", (int)compromisso.TipoCompromisso);
        comando.AdicionarParametro("LOCAL", compromisso.Local);
        comando.AdicionarParametro("LINK", compromisso.Link);
        comando.AdicionarParametro("CONTATO_ID", compromisso.Contato?.Id!);
    }
}
