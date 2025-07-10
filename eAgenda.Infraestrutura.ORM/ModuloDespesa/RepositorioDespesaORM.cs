using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.ORM.ModuloDespesa;

public class RepositorioDespesaORM : IRepositorioDespesa
{
    private readonly eAgendaDbContext contexto;

    public RepositorioDespesaORM(eAgendaDbContext contexto)
    {
        this.contexto = contexto;
    }

    public void CadastrarRegistro(Despesa novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();
        contexto.Despesas.Add(novoRegistro);
    }

    public bool EditarRegistro(Guid idRegistro, Despesa registroEditado)
    {
        Despesa? despesaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (despesaSelecionada is null)
            return false;

        despesaSelecionada.AtualizarRegistro(registroEditado);

        return true;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        Despesa? despesaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (despesaSelecionada is null)
            return false;

        contexto.Despesas.Remove(despesaSelecionada);

        return true;
    }

    public Despesa? SelecionarRegistroPorId(Guid idRegistro)
    {
        return contexto.Despesas.Where(d => d.Id.Equals(idRegistro)).Include(d => d.Categorias).FirstOrDefault();
    }

    public List<Despesa> SelecionarRegistros()
    {
        return [.. contexto.Despesas.Include(d => d.Categorias)];
    }

    public void AdicionarCategoria(Categoria categoria, Despesa despesa)
    {
        categoria.Despesas.Add(despesa);
    }

    public void RemoverCategoria(Categoria categoria, Despesa despesa)
    {
        categoria.Despesas.Remove(despesa);
    }
}
