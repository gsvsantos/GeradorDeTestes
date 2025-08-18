using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de Teste/Provão")]
public class TesteTestes
{
    private static Disciplina disciplinaPadrao = new("Matemática");
    private Materia materiaPadrao = new("Quatro Operações", disciplinaPadrao, EnumSerie.QuartoAnoFundamental) { Id = Guid.NewGuid() };

    private Teste? teste;

    [TestMethod]
    public void Deve_AderirQuestao_Ao_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        Questao questao = new("Quanto é 53 - 38?", materiaPadrao);

        teste.AderirQuestao(questao);

        bool testeContemQuestao = teste.Questoes.Contains(questao);

        Assert.IsTrue(testeContemQuestao);
    }

    [TestMethod]
    public void Deve_AderirQuestoes_Ao_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        List<Questao> novasQuestoes = new(){
            new ("Quanto é 2 + 2?", materiaPadrao),
            new ("Quanto é 53 - 38?", materiaPadrao),
            new ("Quanto é 985 + 15?", materiaPadrao),
            new ("Quanto é 9 / 3?", materiaPadrao),
            new ("Quanto é 30 * 15?", materiaPadrao)
            };

        teste.AderirQuestoes(novasQuestoes);

        List<Questao> questoesEsperadas = novasQuestoes;
        List<Questao> questoesDoTeste = teste.Questoes;

        Assert.AreEqual(questoesDoTeste.Count, questoesEsperadas.Count);
        CollectionAssert.IsSubsetOf(questoesDoTeste, questoesEsperadas);
    }

    [TestMethod]
    public void Deve_RemoverQuestao_Do_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        Questao questao = new("Quanto é 53 - 38?", materiaPadrao);

        teste.AderirQuestao(questao);

        List<Questao> questoesTesteAposAderir = teste.Questoes.ToList();

        teste.RemoverQuestao(questao);

        List<Questao> questoesTesteAposRemover = teste.Questoes.ToList();

        Assert.AreNotEqual(questoesTesteAposAderir.Count, questoesTesteAposRemover.Count);
        CollectionAssert.IsNotSubsetOf(questoesTesteAposAderir, questoesTesteAposRemover);
    }

    [TestMethod]
    public void Deve_AderirMateria_Ao_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        teste.AderirMateria(materiaPadrao);

        bool testeContemQuestao = teste.Materias.Contains(materiaPadrao);

        Assert.IsTrue(testeContemQuestao);
    }

    [TestMethod]
    public void Deve_AderirMaterias_Ao_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        Materia materiaFracoes = new(
            "Frações",
            disciplinaPadrao,
            EnumSerie.QuartoAnoFundamental
        );

        List<Materia> novasMaterias = new(){
            materiaPadrao,
            materiaFracoes
            };

        foreach (Materia m in novasMaterias)
            teste.AderirMateria(m);

        List<Materia> materiasEsperadas = novasMaterias;
        List<Materia> materiasDoTeste = teste.Materias;

        Assert.AreEqual(materiasDoTeste.Count, materiasEsperadas.Count);
        CollectionAssert.IsSubsetOf(materiasDoTeste, materiasEsperadas);
    }

    [TestMethod]
    public void Deve_RemoverMateria_Do_Teste_Corretamente()
    {
        teste = new("Teste de Matemática", disciplinaPadrao, EnumSerie.QuartoAnoFundamental, false, 3);

        teste.AderirMateria(materiaPadrao);

        List<Materia> materiasTesteAposAderir = teste.Materias.ToList();

        teste.RemoverMateria(materiaPadrao);

        List<Materia> materiasTesteAposRemover = teste.Materias.ToList();

        Assert.AreNotEqual(materiasTesteAposAderir.Count, materiasTesteAposRemover.Count);
        CollectionAssert.IsNotSubsetOf(materiasTesteAposAderir, materiasTesteAposRemover);
    }
}
