using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Integracao.ModuloQuestao;

[TestClass]
[TestCategory("Testes de Integra��o de Quest�o")]
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
            .With(d => d.Nome = "Matem�tica")
            .Persist();

        materiaPadrao = Builder<Materia>
            .CreateNew()
            .WithFactory(() => new("Multiplica��o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental))
            .Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Questao_Corretamente()
    {
        Questao novaQuestao = new("1 * 1", materiaPadrao);

        repositorioQuestaoORM.CadastrarRegistro(novaQuestao);

        dbContext.SaveChanges();

        Questao? questaoSelecionada = repositorioQuestaoORM.SelecionarRegistroPorId(novaQuestao.Id);

        Assert.IsNotNull(questaoSelecionada, "N�o conseguiu selecionar a quest�o.");
        Assert.AreEqual(novaQuestao, questaoSelecionada, "A quest�o selecionada n�o corresponde com a quest�o cadastrada.");
        Assert.AreEqual(disciplinaPadrao, questaoSelecionada.Materia.Disciplina, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(materiaPadrao, questaoSelecionada.Materia, "A mat�ria da quest�o selecionada n�o corresponde com a mat�ria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, questaoSelecionada.Materia.Serie, "A s�rie n�o corresponde com a s�rie da mat�ria cadastrada.");
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

        Assert.IsTrue(conseguiuEditar, "N�o conseguiu editar a quest�o.");
        Assert.IsNotNull(questaoSelecionada, "N�o conseguiu selecionar a quest�o.");
        Assert.AreEqual(novaQuestao, questaoSelecionada, "A quest�o selecionada n�o corresponde com a quest�o editada.");
        Assert.AreEqual(questaoEditada.Materia, questaoSelecionada.Materia, "A mat�ria da quest�o selecionada n�o corresponde com a quest�o editada.");
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

        Assert.IsTrue(conseguiuExcluir, "N�o conseguiu excluir a quest�o.");
        Assert.IsNull(questaoSelecionada, "A quest�o ainda est� no banco ap�s exclus�o.");
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
