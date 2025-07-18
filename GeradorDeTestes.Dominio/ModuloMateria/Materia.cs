using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Dominio.ModuloMateria;
public class Materia : EntidadeBase<Materia>
{
    public string Nome { get; set; }
    public Disciplina Disciplina { get; set; }
    public EnumSerie Serie { get; set; }
    public List<Questao> Questoes { get; set; } = new List<Questao>();

    public Materia(string nome, Disciplina disciplina, EnumSerie serie)
    {
        Nome = nome;
        Disciplina = disciplina;
        Serie = serie;
    }
    protected Materia() { }


    public override void AtualizarRegistro(Materia registroEditado)
    {
        Nome = registroEditado.Nome;
        Disciplina = registroEditado.Disciplina;
        Serie = registroEditado.Serie;
    }
}
