using GeradorDeTestes.Dominio.Compartilhado;

namespace GeradorDeTestes.Dominio.ModuloQuestao;

public interface IRepositorioQuestao : IRepositorio<Questao>
{
    public Alternativa? SelecionarAlternativa(Questao questao, Guid idAlternativa);
    public List<Questao> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo);
    public List<Questao> SelecionarNaoFinalizados();
    public void RemoverRegistros(List<Questao> questoes);
}
