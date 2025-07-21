using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Dominio.ModuloTeste;
public interface IRepositorioTeste : IRepositorio<Teste>
{
    public void AtualizarQuantidadePorMateria(Teste teste, Materia materia);
    public List<Teste> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo);
    public List<Teste> SelecionarNaoFinalizados();
    public void RemoverRegistros(List<Teste> testes);
}
