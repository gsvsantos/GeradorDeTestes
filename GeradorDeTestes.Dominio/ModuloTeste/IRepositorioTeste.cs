using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Dominio.ModuloTeste;
public interface IRepositorioTeste : IRepositorio<Teste>
{
    public void AtualizarQuantidadePorMateria(Teste teste, Materia materia);
    public List<Teste> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo);
    public List<Teste> SelecionarNaoFinalizados();
    public List<Questao> SelecionarQuestoesParaProvao(Guid disciplinaId, int quantidade);
    public int RemoverRegistros(List<Teste> testes);
}
