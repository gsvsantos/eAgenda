using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloCategoria;

public class RepositorioCategoriaORM : IRepositorioCategoria
{
    private readonly eAgendaDbContext contexto;

    public RepositorioCategoriaORM(eAgendaDbContext contexto)
    {
        this.contexto = contexto;
    }

    public void CadastrarRegistro(Categoria novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();
        contexto.Categorias.Add(novoRegistro);
    }

    public bool EditarRegistro(Guid idRegistro, Categoria registroEditado)
    {
        Categoria? categoriaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (categoriaSelecionada is null)
            return false;

        categoriaSelecionada.AtualizarRegistro(registroEditado);

        return true;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        Categoria? categoriaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (categoriaSelecionada is null)
            return false;

        contexto.Categorias.Remove(categoriaSelecionada);

        return true;
    }

    public Categoria? SelecionarRegistroPorId(Guid idRegistro)
    {

        return contexto.Categorias.Where(c => c.Id.Equals(idRegistro)).Include(c => c.Despesas).FirstOrDefault();
    }

    public List<Categoria> SelecionarRegistros()
    {
        return [.. contexto.Categorias.Include(c => c.Despesas)];
    }
}
