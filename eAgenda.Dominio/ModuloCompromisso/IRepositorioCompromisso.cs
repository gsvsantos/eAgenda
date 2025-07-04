using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.ModuloCompromisso
{
    public interface IRepositorioCompromisso : IRepositorio<Compromisso>
    {
        public List<Compromisso> SelecionarCompromissosContato(Guid idRegistro);
        bool TemConflito(Compromisso compromisso);
    }
}
