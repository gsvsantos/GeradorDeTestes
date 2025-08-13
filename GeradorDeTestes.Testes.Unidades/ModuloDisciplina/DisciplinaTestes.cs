using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Unidades.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de Unidade de Disciplina")]
public class DisciplinaTestes
{
    private Disciplina? disciplina;

    [TestMethod]
    public void Deve_AderirMateria_ADisciplina_Corretamente()
    {
        // Arrange
        disciplina = new("Matemática");

        Materia materia = new("Quatro Operações", disciplina, EnumSerie.SetimoAnoFundamental);

        // Act
        disciplina.AderirMateria(materia);

        // Assert
        bool disciplinaContemMateria = disciplina.Materias.Contains(materia);

        Assert.IsTrue(disciplinaContemMateria);
    }

    [TestMethod]
    public void Deve_SortearQuestoes_DaDisciplina_Corretamente()
    {
        // Arrange
        disciplina = new Disciplina("Matemática");

        Materia materiaQuatroOperacoes = new(
            "Quatro Operações",
            disciplina,
            EnumSerie.SetimoAnoFundamental
        );

        Materia materiaFracoes = new(
            "Frações",
            disciplina,
            EnumSerie.SetimoAnoFundamental
        );

        materiaQuatroOperacoes.AderirQuestoes(new(){
            new ("Quanto é 2 + 2?", materiaQuatroOperacoes),
            new ("Quanto é 53 - 38?", materiaQuatroOperacoes),
            new ("Quanto é 985 + 15?", materiaQuatroOperacoes),
            new ("Quanto é 9 / 3?", materiaQuatroOperacoes),
            new ("Quanto é 30 * 15?", materiaQuatroOperacoes)
        });

        materiaFracoes.AderirQuestoes(new(){
            new ("Qual é a fração que representa a metade de uma pizza?", materiaFracoes),
            new ("Qual fração representa três partes de um total de quatro partes iguais?", materiaFracoes),
            new ("Qual é o resultado da soma: 1/4 + 1/4?", materiaFracoes),
            new ("Qual é a fração equivalente a 2/4?", materiaFracoes),
            new ("Se você tem uma barra de chocolate dividida em 8 pedaços e come 3, qual fração representa o que você comeu?", materiaFracoes)
        });

        disciplina.AderirMateria(materiaQuatroOperacoes);
        disciplina.AderirMateria(materiaFracoes);

        // Act
        List<Questao> questoesSorteadas = disciplina.ObterQuestoesAleatorias(10, EnumSerie.SetimoAnoFundamental);

        // Assert
        List<Questao> questoesEsperadas = new(
            materiaQuatroOperacoes.Questoes
                .Concat(materiaFracoes.Questoes)
        );

        Assert.AreEqual(questoesSorteadas.Count, questoesEsperadas.Count);
        CollectionAssert.IsSubsetOf(questoesSorteadas, questoesEsperadas);
    }
}
