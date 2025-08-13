using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Unidades.ModuloMateria;

[TestClass]
[TestCategory("Testes de Unidade de Matéria")]
public class MateriaTestes
{
    private Disciplina disciplinaPadrao = new("Matemática");
    private Materia? materia;

    [TestMethod]
    public void Deve_AderirQuestao_AMateria_Corretamente()
    {
        materia = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        Questao questao = new("Quanto é 53 - 38?", materia);

        materia.AderirQuestao(questao);

        bool materiaContemQuestao = materia.Questoes.Contains(questao);

        Assert.IsTrue(materiaContemQuestao);
    }

    [TestMethod]
    public void Deve_AderirQuestoes_AMateria_Corretamente()
    {
        materia = new("Quatro Operações", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        materia.AderirQuestoes([
            new ("Quanto é 2 + 2?", materia),
            new ("Quanto é 53 - 38?", materia),
            new ("Quanto é 985 + 15?", materia),
            new ("Quanto é 9 / 3?", materia),
            new ("Quanto é 30 * 15?", materia)
        ]);

        List<Questao> questoesEsperadas = [.. materia.Questoes];
        List<Questao> questoesDaMateria = materia.Questoes;

        Assert.AreEqual(questoesDaMateria.Count, questoesEsperadas.Count);
        CollectionAssert.IsSubsetOf(questoesDaMateria, questoesEsperadas);
    }
}
