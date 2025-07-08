using eAgenda.Dominio.ModuloContato;
using eAgenda.Infraestrutura.SQLServer.Compartilhado;
using eAgenda.Infraestrutura.SQLServer.Extensions;
using System.Data;

namespace eAgenda.Infraestrutura.SQLServer.ModuloContato;

public class RepositorioContatoSQL : RepositorioBaseSQL<Contato>, IRepositorioContato
{
    protected override string SqlCadastrar => @"INSERT INTO [TBCONTATO]
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
            )";
    protected override string SqlEditar => @"UPDATE [TBCONTATO]	
		    SET
			    [NOME] = @NOME,
			    [EMAIL] = @EMAIL,
			    [TELEFONE] = @TELEFONE,
			    [EMPRESA] = @EMPRESA,
			    [CARGO] = @CARGO
		    WHERE
			    [ID] = @ID";
    protected override string SqlExcluir => @"DELETE FROM [TBCONTATO]
		    WHERE
			    [ID] = @ID";
    protected override string SqlSelecionarPorId => @"SELECT 
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
    protected override string SqlSelecionarTodos => @"SELECT
                [ID],
                [NOME],
                [EMAIL],
                [TELEFONE],
                [EMPRESA],
                [CARGO]
            FROM
                [TBCONTATO]";
    private static string SqlVerificarVinculos => @"SELECT COUNT(*)
            FROM 
                [TBCOMPROMISSO]
            WHERE
	            [CONTATO_ID] = @ID";

    public RepositorioContatoSQL(IDbConnection conexaoComBanco) : base(conexaoComBanco) { }

    public bool ExistePorEmailOuTelefone(string email, string telefone, Guid? ignorarId = null)
    {
        throw new NotImplementedException();
    }

    public bool PossuiCompromissosVinculados(Guid id)
    {
        IDbCommand comandoVerificacao = conexaoComBanco.CreateCommand();
        comandoVerificacao.CommandText = SqlVerificarVinculos;

        comandoVerificacao.AdicionarParametro("ID", id);

        conexaoComBanco.Open();

        int quantidadeConflitos = Convert.ToInt32(comandoVerificacao.ExecuteScalar());

        conexaoComBanco.Close();

        return quantidadeConflitos >= 1;
    }

    protected override Contato ConverterParaRegistro(IDataReader leitor)
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

    protected override void ConfigurarParametrosRegistro(Contato contato, IDbCommand comando)
    {
        comando.AdicionarParametro("ID", contato.Id);
        comando.AdicionarParametro("NOME", contato.Nome);
        comando.AdicionarParametro("EMAIL", contato.Email);
        comando.AdicionarParametro("TELEFONE", contato.Telefone);
        comando.AdicionarParametro("EMPRESA", contato.Empresa!);
        comando.AdicionarParametro("CARGO", contato.Cargo!);
    }
}
