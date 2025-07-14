using eAgenda.Dominio.ModuloDespesa;
using eAgenda.WebApp.Models;

namespace eAgenda.WebApp.Extensions;

public static class DespesaExtensions
{
    public static Despesa ParaEntidade(this FormularioDespesaViewModel formularioVM)
    {
        DateTime dataUtc = formularioVM.DataOcorrencia.Kind == DateTimeKind.Utc
                         ? formularioVM.DataOcorrencia
                         : formularioVM.DataOcorrencia.ToUniversalTime();

        return new Despesa(
            formularioVM.Titulo,
            formularioVM.Descricao,
            dataUtc,
            formularioVM.Valor,
            formularioVM.FormaPagamento);
    }

    public static DetalhesDespesaViewModel ParaDetalhesVM(this Despesa despesa)
    {
        return new DetalhesDespesaViewModel(
                despesa.Id,
                despesa.Titulo,
                despesa.Descricao,
                despesa.DataOcorrencia,
                despesa.Valor,
                despesa.FormaPagamento,
                despesa.Categorias);
    }
}