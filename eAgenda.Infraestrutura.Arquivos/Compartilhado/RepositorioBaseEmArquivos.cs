using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Infraestrutura.Arquivos.Compartilhado;

public abstract class RepositorioBaseEmArquivo<Tipo> where Tipo : EntidadeBase<Tipo>
{
    protected ContextoDados contexto;
    protected List<Tipo> registros = [];

    protected RepositorioBaseEmArquivo(ContextoDados contexto)
    {
        this.contexto = contexto;

        registros = ObterRegistros();
    }

    public virtual void CadastrarRegistro(Tipo novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();
        registros.Add(novoRegistro);

        contexto.Salvar();
    }

    protected abstract List<Tipo> ObterRegistros();

    public bool EditarRegistro(Guid idRegistro, Tipo registroEditado)
    {
        Tipo? registroSelecionado = SelecionarRegistroPorId(idRegistro);

        if (registroSelecionado is null)
            return true;

        registroSelecionado.AtualizarRegistro(registroEditado);

        contexto.Salvar();

        return true;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        Tipo? registroSelecionado = SelecionarRegistroPorId(idRegistro);

        if (registroSelecionado is null)
            return true;

        registros.Remove(registroSelecionado);

        contexto.Salvar();

        return true;
    }

    public List<Tipo> SelecionarRegistros()
    {
        return registros;
    }

    public Tipo? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Find((registro) => registro.Id.Equals(idRegistro));
    }
}