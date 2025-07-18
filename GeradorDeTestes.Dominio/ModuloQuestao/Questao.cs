using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;

namespace GeradorDeTestes.Dominio.ModuloQuestao;
public class Questao : EntidadeBase<Questao>
{
    public string Enunciado { get; set; }
    public Materia Materia { get; set; }
    public List<Alternativa> Alternativas { get; set; } = new List<Alternativa>();
    public List<Teste> Testes { get; set; } = new List<Teste>();

    public Questao(Materia materia, string enunciado, List<Alternativa> alternativas)
    {
        Materia = materia;
        Enunciado = enunciado;
        Alternativas = alternativas;
    }
    protected Questao() { }

    public override void AtualizarRegistro(Questao registroEditado)
    {
        Materia = registroEditado.Materia;
        Enunciado = registroEditado.Enunciado;
    }
}
