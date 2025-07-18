using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Dominio.ModuloTeste;
public class Teste : EntidadeBase<Teste>
{
    public string Titulo { get; set; }
    public Disciplina Disciplina { get; set; }
    public EnumSerie Serie { get; set; }
    public Materia? Materia { get; set; }
    public bool EhRecuperacao { get; set; }
    public int QuantidadeQuestoes { get; set; }
    public List<Questao> Questoes { get; set; } = new List<Questao>();

    public Teste(string titulo, Disciplina disciplina, EnumSerie serie, Materia? materia, bool ehRecuperacao, int quantidadeQuestoes)
    {
        Titulo = titulo;
        Disciplina = disciplina;
        Serie = serie;
        Materia = materia;
        EhRecuperacao = ehRecuperacao;
        QuantidadeQuestoes = quantidadeQuestoes;
    }
    protected Teste() { }

    public void AderirQuestao(Questao questao)
    {
        Questoes.Add(questao);
    }

    public void RemoverQuestao(Questao questao)
    {
        Questoes.Remove(questao);
    }

    public override void AtualizarRegistro(Teste registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Disciplina = registroEditado.Disciplina;
        Serie = registroEditado.Serie;
        Materia = registroEditado.Materia;
        EhRecuperacao = registroEditado.EhRecuperacao;
        QuantidadeQuestoes = registroEditado.QuantidadeQuestoes;
    }
}
