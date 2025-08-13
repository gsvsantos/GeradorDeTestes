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
        disciplina = new("Matem�tica");

        Materia materia = new("Quatro Opera��es", disciplina, EnumSerie.SetimoAnoFundamental);

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
        disciplina = new Disciplina("Matem�tica");

        Materia materiaQuatroOperacoes = new(
            "Quatro Opera��es",
            disciplina,
            EnumSerie.SetimoAnoFundamental
        );

        Materia materiaFracoes = new(
            "Fra��es",
            disciplina,
            EnumSerie.SetimoAnoFundamental
        );

        materiaQuatroOperacoes.AderirQuestoes(new(){
            new ("Quanto � 2 + 2?", materiaQuatroOperacoes),
            new ("Quanto � 53 - 38?", materiaQuatroOperacoes),
            new ("Quanto � 985 + 15?", materiaQuatroOperacoes),
            new ("Quanto � 9 / 3?", materiaQuatroOperacoes),
            new ("Quanto � 30 * 15?", materiaQuatroOperacoes)
        });

        materiaFracoes.AderirQuestoes(new(){
            new ("Qual � a fra��o que representa a metade de uma pizza?", materiaFracoes),
            new ("Qual fra��o representa tr�s partes de um total de quatro partes iguais?", materiaFracoes),
            new ("Qual � o resultado da soma: 1/4 + 1/4?", materiaFracoes),
            new ("Qual � a fra��o equivalente a 2/4?", materiaFracoes),
            new ("Se voc� tem uma barra de chocolate dividida em 8 peda�os e come 3, qual fra��o representa o que voc� comeu?", materiaFracoes)
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
