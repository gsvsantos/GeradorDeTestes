using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Integracao.ModuloQuestao;

[TestClass]
[TestCategory("Testes de Integração de Questão")]
public sealed class RepositorioQuestaoORMTestes : TestFixture
{
    private Disciplina disciplinaPadrao = null!;
    private Materia materiaPadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        disciplinaPadrao = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .Persist();

        materiaPadrao = Builder<Materia>
            .CreateNew()
            .WithFactory(() => new("Multiplicação", disciplinaPadrao, EnumSerie.SetimoAnoFundamental))
            .Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Questao_Corretamente()
    {
        Questao novaQuestao = new("1 * 1", materiaPadrao);

        repositorioQuestaoORM.CadastrarRegistro(novaQuestao);

        dbContext.SaveChanges();

        Questao? questaoSelecionada = repositorioQuestaoORM.SelecionarRegistroPorId(novaQuestao.Id);

        Assert.IsNotNull(questaoSelecionada, "Não conseguiu selecionar a questão.");
        Assert.AreEqual(novaQuestao, questaoSelecionada, "A questão selecionada não corresponde com a questão cadastrada.");
        Assert.AreEqual(disciplinaPadrao, questaoSelecionada.Materia.Disciplina, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(materiaPadrao, questaoSelecionada.Materia, "A matéria da questão selecionada não corresponde com a matéria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, questaoSelecionada.Materia.Serie, "A série não corresponde com a série da matéria cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Questao_Corretamente()
    {
        Questao novaQuestao = new("1 * 1", materiaPadrao);

        repositorioQuestaoORM.CadastrarRegistro(novaQuestao);

        dbContext.SaveChanges();

        Materia novaMateria = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "Soma")
            .With(m => m.Disciplina = disciplinaPadrao)
            .With(m => m.Serie = EnumSerie.SetimoAnoFundamental)
            .Persist();

        Questao questaoEditada = new("34 + 35", novaMateria);

        bool conseguiuEditar = repositorioQuestaoORM.EditarRegistro(novaQuestao.Id, questaoEditada);

        dbContext.SaveChanges();

        Questao? questaoSelecionada = repositorioQuestaoORM.SelecionarRegistroPorId(novaQuestao.Id);

        Assert.IsTrue(conseguiuEditar, "Não conseguiu editar a questão.");
        Assert.IsNotNull(questaoSelecionada, "Não conseguiu selecionar a questão.");
        Assert.AreEqual(novaQuestao, questaoSelecionada, "A questão selecionada não corresponde com a questão editada.");
        Assert.AreEqual(questaoEditada.Materia, questaoSelecionada.Materia, "A matéria da questão selecionada não corresponde com a questão editada.");
    }

    [TestMethod]
    public void Deve_Excluir_Questao_Corretamente()
    {
        Questao novaQuestao = new("1 * 1", materiaPadrao);

        repositorioQuestaoORM.CadastrarRegistro(novaQuestao);

        dbContext.SaveChanges();

        bool conseguiuExcluir = repositorioQuestaoORM.ExcluirRegistro(novaQuestao.Id);

        dbContext.SaveChanges();

        Questao? questaoSelecionada = repositorioQuestaoORM.SelecionarRegistroPorId(novaQuestao.Id);

        Assert.IsTrue(conseguiuExcluir, "Não conseguiu excluir a questão.");
        Assert.IsNull(questaoSelecionada, "A questão ainda está no banco após exclusão.");
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_As_Questoes_Corretamente()
    {
        List<Disciplina> novasDisciplinas = Builder<Disciplina>.CreateListOfSize(3).Persist().ToList();

        List<Materia> novasMaterias = new List<Materia>();

        for (int i = 0; i < novasDisciplinas.Count; i++)
        {
            Materia materia = Builder<Materia>
                .CreateNew()
                .With(m => m.Disciplina = novasDisciplinas[i])
                .Persist();

            novasMaterias.Add(materia);
        }

        List<Questao> novasQuestoes = new()
        {
            new Questao("1 + 1", novasMaterias[0]),
            new Questao("1 / 2", novasMaterias[1]),
            new Questao("1 * 3", novasMaterias[2])
        };

        foreach (Questao questao in novasQuestoes)
        {
            repositorioQuestaoORM.CadastrarRegistro(questao);
        }

        dbContext.SaveChanges();

        List<Questao> questoesExistentesOrganizadas = repositorioQuestaoORM.SelecionarRegistros();
        List<Questao> novasQuestoesOrganizadas = novasQuestoes.OrderBy(q => q.Enunciado).ToList();

        Assert.AreEqual(novasMaterias.Count, questoesExistentesOrganizadas.Count);
        CollectionAssert.AreEquivalent(novasQuestoesOrganizadas, questoesExistentesOrganizadas);
    }
}
