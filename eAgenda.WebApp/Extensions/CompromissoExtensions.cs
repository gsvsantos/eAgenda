﻿using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.WebApp.Models;

namespace eAgenda.WebApp.Extensions;

public static class CompromissoExtensions
{
    public static Compromisso ParaEntidade(this FormularioCompromissoViewModel formularioVM, Contato contato)
    {
        return new(
            formularioVM.Assunto,
            formularioVM.DataOcorrencia,
            formularioVM.HoraInicio,
            formularioVM.HoraTermino,
            formularioVM.TipoCompromisso,
            formularioVM.Local!,
            formularioVM.Link!,
            contato);
    }

    public static DetalhesCompromissoViewModel ParaDetalhesVM(this Compromisso compromisso)
    {
        return new(
            compromisso.Id,
            compromisso.Assunto,
            compromisso.DataOcorrencia,
            compromisso.HoraInicio,
            compromisso.HoraTermino,
            compromisso.TipoCompromisso,
            compromisso.Local,
            compromisso.Link,
            compromisso.Contato);
    }
}
