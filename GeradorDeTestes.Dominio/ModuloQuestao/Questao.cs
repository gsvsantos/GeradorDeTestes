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

    public void AderirAlternativa(Alternativa alternativa)
    {
        Alternativas.Add(alternativa);
    }

    public void RemoverAlternativa(Alternativa alternativa)
    {
        Alternativas.Remove(alternativa);
    }

    public void AderirTeste(Teste teste)
    {
        Testes.Add(teste);
    }

    public void RemoverTeste(Teste teste)
    {
        Testes.Remove(teste);
    }

    public bool EstaCompleta()
    {
        bool temMinimoDuas = Alternativas.Count >= 2;
        bool temUmaCorreta = Alternativas.Count(a => a.EstaCorreta) == 1;

        return temMinimoDuas && temUmaCorreta;
    }

    public override void AtualizarRegistro(Questao registroEditado)
    {
        Materia = registroEditado.Materia;
        Enunciado = registroEditado.Enunciado;
    }
}
