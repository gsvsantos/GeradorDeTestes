namespace GeradorDeTestes.Dominio.ModuloQuestao;

public class Alternativa
{
    public Guid Id { get; set; }
    public string Texto { get; set; }
    public Questao Questao { get; set; }
    public bool EstaCorreta { get; set; }

    public Alternativa(string texto, Questao questao)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Questao = questao;
    }
    protected Alternativa() { }
}
