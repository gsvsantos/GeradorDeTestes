using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;

namespace GeradorDeTestes.Dominio.ModuloQuestao;
public class Questao : EntidadeBase<Questao>
{
    public string Enunciado { get; set; }
    public Materia Materia { get; set; }
    public bool Finalizado { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<Alternativa> Alternativas { get; set; } = new List<Alternativa>();
    public List<Teste> Testes { get; set; } = new List<Teste>();

    public Questao() { }
    public Questao(string enunciado, Materia materia) : this()
    {
        Enunciado = enunciado;
        Materia = materia;
    }
    public void AderirAlternativas(List<Alternativa> alternativas)
    {
        if (Alternativas.Any(alternativas.Contains))
            return;

        Alternativas.AddRange(alternativas);
    }

    public void AderirAlternativa(Alternativa alternativa)
    {
        Alternativas.Add(alternativa);
    }

    public void RemoverAlternativa(Alternativa alternativa)
    {
        Alternativas.Remove(alternativa);
    }

    public override void AtualizarRegistro(Questao registroEditado)
    {
        Materia = registroEditado.Materia;
        Enunciado = registroEditado.Enunciado;
    }
}
