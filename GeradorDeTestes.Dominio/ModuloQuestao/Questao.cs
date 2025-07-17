using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Dominio.ModuloQuestao;
public class Questao : EntidadeBase<Questao>
{
    public Materia Materia { get; set; }
    public string Enunciado { get; set; }
    public List<Alternativa> Alternativas { get; set; } = new List<Alternativa>();

    public Questao(Materia materia, string enunciado, List<Alternativa> alternativas)
    {
        Materia = materia;
        Enunciado = enunciado;
        foreach (Alternativa a in alternativas)
        {
            Alternativas.Add(a);
        }
    }

    public override void AtualizarRegistro(Questao registroEditado)
    {
        Materia = registroEditado.Materia;
        Enunciado = registroEditado.Enunciado;
    }
}
