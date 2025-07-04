using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Infraestrutura.Arquivos.Compartilhado;

namespace eAgenda.Infra.Dados.Arquivo.ModuloCompromisso
{
    public class RepositorioCompromissoEmArquivo : RepositorioBaseEmArquivo<Compromisso>, IRepositorioCompromisso
    {
        public RepositorioCompromissoEmArquivo(ContextoDados contexto) : base(contexto)
        {
        }

        protected override List<Compromisso> ObterRegistros()
        {
            return contexto.Compromissos;
        }

        public bool TemConflito(Compromisso compromisso)
        {
            return registros.Any(c =>
                c.Id != compromisso.Id &&
                c.DataOcorrencia == compromisso.DataOcorrencia &&
                HorariosConflitam(c, compromisso)
            );
        }

        private bool HorariosConflitam(Compromisso c1, Compromisso c2)
        {
            return
                (c2.HoraInicio >= c1.HoraInicio && c2.HoraInicio < c1.HoraTermino) ||
                (c2.HoraTermino > c1.HoraInicio && c2.HoraTermino <= c1.HoraTermino) ||
                (c2.HoraInicio <= c1.HoraInicio && c2.HoraTermino >= c1.HoraTermino);
        }

        public List<Compromisso> SelecionarCompromissosContato(Guid idRegistro)
        {
            throw new NotImplementedException();
        }
    }
}
