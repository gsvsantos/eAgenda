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
        foreach (Tipo item in registros)
        {
            if (item.Id == idRegistro)
            {
                item.AtualizarRegistro(registroEditado);

                contexto.Salvar();

                return true;
            }
        }

        return false;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        Tipo registroSelecionado = SelecionarRegistroPorId(idRegistro);

        if (registroSelecionado != null)
        {
            registros.Remove(registroSelecionado);

            contexto.Salvar();

            return true;
        }

        return false;
    }

    public List<Tipo> SelecionarRegistros()
    {
        return registros;
    }

    public Tipo SelecionarRegistroPorId(Guid idRegistro)
    {
        foreach (Tipo item in registros)
        {
            if (item.Id == idRegistro)
                return item;
        }

        return null!;
    }
}