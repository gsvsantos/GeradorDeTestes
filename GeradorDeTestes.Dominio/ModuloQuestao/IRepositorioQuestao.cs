using GeradorDeTestes.Dominio.Compartilhado;

namespace GeradorDeTestes.Dominio.ModuloQuestao;

public interface IRepositorioQuestao : IRepositorio<Questao>
{
    public Alternativa? SelecionarAlternativa(Questao questao, Guid idAlternativa);
}
