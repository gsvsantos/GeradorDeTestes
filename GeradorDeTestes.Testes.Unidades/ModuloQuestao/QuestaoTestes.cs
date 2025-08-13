using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de Questão")]
public class QuestaoTestes
{
    private Materia materiaPadrao = new("Quatro Operações", new("Matemática"), EnumSerie.QuartoAnoFundamental);
    private Questao? questao;

    [TestMethod]
    public void Deve_AderirAlternativa_AQuestao_Corretamente()
    {
        questao = new("Quanto é 53 - 38?", materiaPadrao);

        Alternativa alternativa = new("25", questao) { EstaCorreta = true };

        questao.AderirAlternativa(alternativa);

        bool questaoContemAlternativa = questao.Alternativas.Contains(alternativa);

        Assert.IsTrue(questaoContemAlternativa);
    }

    [TestMethod]
    public void Deve_AderirAlternativas_AQuestao_Corretamente()
    {
        questao = new("Quanto é 53 - 38?", materiaPadrao);

        List<Alternativa> novasAlternativas = new(){
            new("32", questao),
            new("61", questao),
            new("12", questao),
            new("25", questao) {EstaCorreta = true}
        };

        questao.AderirAlternativas(novasAlternativas);

        List<Alternativa> alternativasEsperadas = novasAlternativas;
        List<Alternativa> alternativasDaMateria = questao.Alternativas;

        Assert.AreEqual(alternativasDaMateria.Count, alternativasEsperadas.Count);
        CollectionAssert.IsSubsetOf(alternativasDaMateria, alternativasEsperadas);
    }

    [TestMethod]
    public void Deve_RemoverAlternativa_Da_Questao_Corretamente()
    {
        questao = new("Quanto é 53 - 38?", materiaPadrao);

        Alternativa alternativa = new("25", questao) { EstaCorreta = true };

        questao.AderirAlternativa(alternativa);

        List<Alternativa> alternativasQuestaoPosAderir = questao.Alternativas.ToList();

        questao.RemoverAlternativa(alternativa);

        List<Alternativa> alternativasQuestaoAposRemover = questao.Alternativas.ToList();

        Assert.AreNotEqual(alternativasQuestaoPosAderir.Count, alternativasQuestaoAposRemover.Count);
        CollectionAssert.IsNotSubsetOf(alternativasQuestaoPosAderir, alternativasQuestaoAposRemover);
    }
}
