using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Dominio.ModuloQuestao;

public interface IGeradorQuestoes
{
    public Task<List<Questao>> GerarQuestoesAsync(Materia materia, int quantidade);
}
