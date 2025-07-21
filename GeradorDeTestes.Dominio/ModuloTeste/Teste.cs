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
    public List<Materia> Materias { get; set; } = new List<Materia>();
    public bool EhProvao { get; set; }
    public int QuantidadeQuestoes { get; set; }
    public List<Questao> Questoes { get; set; } = new List<Questao>();
    public List<TesteMateriaQuantidade> QuantidadesPorMateria { get; set; } = new();

    public Teste() { }
    public Teste(string titulo, Disciplina disciplina, EnumSerie serie, bool ehProvao, int quantidadeQuestoes) : this()
    {
        Titulo = titulo;
        Disciplina = disciplina;
        Serie = serie;
        EhProvao = ehProvao;
        QuantidadeQuestoes = quantidadeQuestoes;
    }

    public void AderirQuestao(Questao questao)
    {
        if (!Questoes.Any(q => q.Id == questao.Id))
        {
            Questoes.Add(questao);
        }
    }

    public void RemoverQuestao(Questao questao)
    {
        Questoes.Remove(questao);
    }

    public void AderirMateria(Materia materia)
    {
        Materias.Add(materia);
    }

    public void RemoverMateria(Materia materia)
    {
        Materias.Remove(materia);
    }

    public override void AtualizarRegistro(Teste registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Disciplina = registroEditado.Disciplina;
        Serie = registroEditado.Serie;
        EhProvao = registroEditado.EhProvao;
        QuantidadeQuestoes = registroEditado.QuantidadeQuestoes;
    }
}
