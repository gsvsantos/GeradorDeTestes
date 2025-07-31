using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.Testes.Integracao.ModuloQuestao;

[TestClass]
[TestCategory("Testes de Integração de Questão")]
public class RepositorioQuestaoORMTestes : TestFixture
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
            .With(m => m.Nome = "Multiplicação")
            .With(m => m.Disciplina = disciplinaPadrao)
            .With(m => m.Serie = EnumSerie.SetimoAnoFundamental)
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
        Assert.AreEqual(novaQuestao.Id, questaoSelecionada.Id, "A questão selecionada não condiz com a questão cadastrada.");
        Assert.AreEqual(novaQuestao.Enunciado, questaoSelecionada.Enunciado, "A questão selecionada não condiz com a questão cadastrada.");
        Assert.AreEqual(disciplinaPadrao.Id, questaoSelecionada.Materia.Disciplina.Id, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(disciplinaPadrao.Nome, questaoSelecionada.Materia.Disciplina.Nome, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(materiaPadrao.Id, questaoSelecionada.Materia.Id, "A matéria da questão selecionada não condiz com a matéria cadastrada.");
        Assert.AreEqual(materiaPadrao.Nome, questaoSelecionada.Materia.Nome, "A matéria da questão selecionada não condiz com a matéria cadastrada.");
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
        Assert.AreEqual(novaQuestao.Id, questaoSelecionada.Id, "A questão selecionada não condiz com a questão cadastrada.");
        Assert.AreEqual(questaoEditada.Enunciado, questaoSelecionada.Enunciado, "O enunciado não corresponde ao editado.");
        Assert.AreEqual(questaoEditada.Materia.Disciplina.Id, questaoSelecionada.Materia.Disciplina.Id, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(questaoEditada.Materia.Disciplina.Nome, questaoSelecionada.Materia.Disciplina.Nome, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(questaoEditada.Materia.Id, questaoSelecionada.Materia.Id, "A matéria da questão selecionada não condiz com a matéria cadastrada.");
        Assert.AreEqual(questaoEditada.Materia.Nome, questaoSelecionada.Materia.Nome, "A matéria da questão selecionada não condiz com a matéria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, questaoSelecionada.Materia.Serie, "A série não corresponde com a série da matéria cadastrada.");
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

        List<Questao> questoesExistentesOrganizadas = repositorioQuestaoORM.SelecionarRegistros().OrderBy(m => m.Enunciado).ToList();
        List<Questao> novasQuestoesOrganizadas = novasQuestoes.OrderBy(m => m.Enunciado).ToList();

        Assert.AreEqual(novasMaterias.Count, questoesExistentesOrganizadas.Count);
        for (int i = 0; i < novasQuestoesOrganizadas.Count; i++)
        {
            Questao novaQuestao = novasQuestoesOrganizadas[i];
            Questao questaoExistente = questoesExistentesOrganizadas[i];

            Assert.AreEqual(novaQuestao.Enunciado, questaoExistente.Enunciado, $"Nome incorreto para a questão '{novaQuestao.Enunciado}'.");
            Assert.AreEqual(novaQuestao.Materia.Serie, questaoExistente.Materia.Serie, $"Série incorreta para a matéria da questão '{novaQuestao.Enunciado}'.");
            Assert.IsNotNull(questaoExistente.Materia.Disciplina, $"Disciplina não carregada para a matéria da questão '{novaQuestao.Enunciado}'.");
            Assert.AreEqual(novaQuestao.Materia.Disciplina.Id, questaoExistente.Materia.Disciplina.Id, $"Disciplina incorreta para a matéria da questão '{novaQuestao.Enunciado}'.");
            Assert.AreEqual(novaQuestao.Materia.Disciplina.Nome, questaoExistente.Materia.Disciplina.Nome, $"Nome da disciplina incorreto para a matéria da questão '{novaQuestao.Enunciado}'.");
        }
    }
}
