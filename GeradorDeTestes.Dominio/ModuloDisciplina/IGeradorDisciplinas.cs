namespace GeradorDeTestes.Dominio.ModuloDisciplina;
public interface IGeradorDisciplinas
{
    public Task<List<Disciplina>> GerarDisciplinasAsync(int quantidade, List<Disciplina> disciplinasExistentes);
}
