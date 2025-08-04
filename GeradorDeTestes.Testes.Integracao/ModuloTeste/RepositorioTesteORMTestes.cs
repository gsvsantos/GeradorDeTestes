using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;

namespace GeradorDeTestes.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integra��o de Teste/Prov�o")]
public sealed class RepositorioTesteORMTestes : TestFixture
{
    private Disciplina disciplinaPadrao = null!;
    private Materia materiaMultiplicacao = null!;
    private Materia materiaSoma = null!;
    private Materia materiaDivisao = null!;
    private List<Questao> questoesTestePadrao = [];
    private List<Questao> questoesProvaoPadrao = [];
    private List<Questao> questoesMultiplicacao = [];

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        disciplinaPadrao = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matem�tica")
            .Persist();

        materiaMultiplicacao = Builder<Materia>
            .CreateNew()
            .WithFactory(() => new("Multiplica��o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental))
            .Persist();

        materiaSoma = Builder<Materia>
           .CreateNew()
           .WithFactory(() => new("Soma", disciplinaPadrao, EnumSerie.SetimoAnoFundamental))
           .Persist();

        materiaDivisao = Builder<Materia>
           .CreateNew()
           .WithFactory(() => new("Divis�o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental))
           .Persist();

        List<Questao> questoesDivisao = CriarQuestoesComMateria(quantidade: 4, materiaDivisao);

        questoesMultiplicacao = CriarQuestoesComMateria(quantidade: 4, materiaMultiplicacao);

        List<Questao> questoesSoma = CriarQuestoesComMateria(quantidade: 4, materiaSoma);

        questoesTestePadrao = questoesMultiplicacao.Take(3).ToList();

        questoesProvaoPadrao = dbContext.Questoes
        .Where(q => q.Materia.Disciplina.Id == disciplinaPadrao.Id)
        .OrderBy(q => Guid.NewGuid())
        .Take(5)
        .ToList();
    }

    [TestMethod]
    public void Deve_Cadastrar_Teste_Corretamente()
    {
        Teste novoTeste = new("Teste de Matem�tica", disciplinaPadrao, EnumSerie.SextoAnoFundamental, false, 3);

        repositorioTesteORM.CadastrarRegistro(novoTeste);

        dbContext.SaveChanges();

        Teste? testeSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoTeste.Id);

        Assert.IsNotNull(testeSelecionado, "N�o conseguiu selecionar o teste.");
        Assert.AreEqual(novoTeste, testeSelecionado, "A quest�o selecionada n�o corresponde com o teste cadastrado.");
        Assert.AreEqual(disciplinaPadrao, novoTeste.Disciplina, "A disciplina n�o corresponde com a disciplina do teste cadastrado.");
        Assert.AreEqual(EnumSerie.SextoAnoFundamental, novoTeste.Serie, "A s�rie n�o corresponde com a s�rie do teste cadastrado.");
    }

    [TestMethod]
    public void Deve_Cadastrar_Provao_Corretamente()
    {
        Teste novoProvao = new("Prov�o de Matem�tica", disciplinaPadrao, EnumSerie.SetimoAnoFundamental, true, 5);

        repositorioTesteORM.CadastrarRegistro(novoProvao);

        dbContext.SaveChanges();

        Teste? provaoSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoProvao.Id);

        Assert.IsNotNull(provaoSelecionado, "N�o conseguiu selecionar o teste.");
        Assert.AreEqual(novoProvao, provaoSelecionado, "A quest�o selecionada n�o corresponde com o teste cadastrado.");
        Assert.AreEqual(disciplinaPadrao, novoProvao.Disciplina, "A disciplina n�o corresponde com a disciplina do teste cadastrado.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, novoProvao.Serie, "A s�rie n�o corresponde com a s�rie do teste cadastrado.");
    }

    [TestMethod]
    public void Deve_Gerar_Teste_Corretamente()
    {
        Teste novoTeste = new("Teste de Matem�tica", disciplinaPadrao, EnumSerie.SextoAnoFundamental, false, 3);

        repositorioTesteORM.CadastrarRegistro(novoTeste);

        dbContext.SaveChanges();

        novoTeste.AderirMateria(materiaMultiplicacao);

        foreach (Questao questao in questoesTestePadrao)
            novoTeste.AderirQuestao(questao);

        novoTeste.Finalizado = true;

        repositorioTesteORM.AtualizarRegistro(novoTeste);

        dbContext.SaveChanges();

        Teste? testeSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoTeste.Id);

        Assert.IsNotNull(testeSelecionado, "N�o conseguiu selecionar o teste.");
        Assert.AreEqual(novoTeste, testeSelecionado, "A quest�o selecionada n�o corresponde com o teste cadastrado.");
        Assert.AreEqual(disciplinaPadrao, novoTeste.Disciplina, "A disciplina n�o corresponde com a disciplina do teste cadastrado.");
        Assert.AreEqual(EnumSerie.SextoAnoFundamental, novoTeste.Serie, "A s�rie n�o corresponde com a s�rie do teste cadastrado.");
        Assert.AreEqual(3, testeSelecionado.Questoes.Count);
        Assert.IsTrue(testeSelecionado.Questoes.All(q => q.Materia.Id.Equals(materiaMultiplicacao.Id)));

    }

    [TestMethod]
    public void Deve_Gerar_Provao_Corretamente()
    {
        Teste novoProvao = new("Prov�o de Matem�tica", disciplinaPadrao, EnumSerie.SetimoAnoFundamental, true, 5);

        repositorioTesteORM.CadastrarRegistro(novoProvao);

        dbContext.SaveChanges();

        foreach (Questao questao in questoesProvaoPadrao)
        {
            novoProvao.AderirQuestao(questao);

            if (!novoProvao.Materias.Any(m => m.Id.Equals(questao.Materia.Id)))
                novoProvao.AderirMateria(questao.Materia);
        }

        novoProvao.Finalizado = true;

        repositorioTesteORM.AtualizarRegistro(novoProvao);

        dbContext.SaveChanges();

        Teste? provaoSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoProvao.Id);

        Assert.IsNotNull(provaoSelecionado, "N�o conseguiu selecionar o teste.");
        Assert.AreEqual(novoProvao, provaoSelecionado, "A quest�o selecionada n�o corresponde com o teste cadastrado.");
        Assert.AreEqual(disciplinaPadrao, novoProvao.Disciplina, "A disciplina n�o corresponde com a disciplina do teste cadastrado.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, novoProvao.Serie, "A s�rie n�o corresponde com a s�rie do teste cadastrado.");
        Assert.AreEqual(5, provaoSelecionado.Questoes.Count);
        Assert.IsTrue(provaoSelecionado.Materias.Count > 1);
    }

    [TestMethod]
    public void Deve_Excluir_Teste_Corretamente()
    {
        Teste novoTeste = new("Teste de Matem�tica", disciplinaPadrao, EnumSerie.SextoAnoFundamental, false, 3);

        repositorioTesteORM.CadastrarRegistro(novoTeste);

        dbContext.SaveChanges();

        novoTeste.AderirMateria(materiaMultiplicacao);

        foreach (Questao questao in questoesTestePadrao)
            novoTeste.AderirQuestao(questao);

        novoTeste.Finalizado = true;

        repositorioTesteORM.AtualizarRegistro(novoTeste);

        dbContext.SaveChanges();

        bool conseguiuExcluir = repositorioTesteORM.ExcluirRegistro(novoTeste.Id);
        dbContext.SaveChanges();

        Teste? testeSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoTeste.Id);

        Assert.IsTrue(conseguiuExcluir, "N�o conseguiu excluir o teste.");
        Assert.IsNull(testeSelecionado, "O teste ainda est� no banco ap�s exclus�o.");
    }

    [TestMethod]
    public void Deve_Excluir_Provao_Corretamente()
    {
        Teste novoProvao = new("Prov�o de Matem�tica", disciplinaPadrao, EnumSerie.SetimoAnoFundamental, true, 5);

        repositorioTesteORM.CadastrarRegistro(novoProvao);

        dbContext.SaveChanges();

        foreach (Questao questao in questoesProvaoPadrao)
        {
            novoProvao.AderirQuestao(questao);

            if (!novoProvao.Materias.Any(m => m.Id.Equals(questao.Materia.Id)))
                novoProvao.AderirMateria(questao.Materia);
        }

        novoProvao.Finalizado = true;

        repositorioTesteORM.AtualizarRegistro(novoProvao);

        dbContext.SaveChanges();

        bool conseguiuExcluir = repositorioTesteORM.ExcluirRegistro(novoProvao.Id);

        dbContext.SaveChanges();

        Teste? provaoSelecionado = repositorioTesteORM.SelecionarRegistroPorId(novoProvao.Id);

        Assert.IsTrue(conseguiuExcluir, "N�o conseguiu excluir o prov�o.");
        Assert.IsNull(provaoSelecionado, "O prov�o ainda est� no banco ap�s exclus�o.");
    }

    [TestMethod]
    public void Deve_Selecionar_Testes_Corretamente()
    {
        Teste novoTeste = new("Teste de Matem�tica", disciplinaPadrao, EnumSerie.SextoAnoFundamental, false, 3);

        novoTeste.AderirMateria(materiaMultiplicacao);

        foreach (Questao questao in questoesTestePadrao)
            novoTeste.AderirQuestao(questao);

        novoTeste.Finalizado = true;


        Teste novoProvao = new("Prov�o de Matem�tica", disciplinaPadrao, EnumSerie.SetimoAnoFundamental, true, 5);

        foreach (Questao questao in questoesProvaoPadrao)
        {
            novoProvao.AderirQuestao(questao);

            if (!novoProvao.Materias.Any(m => m.Id.Equals(questao.Materia.Id)))
                novoProvao.AderirMateria(questao.Materia);
        }

        novoProvao.Finalizado = true;

        List<Teste> registrosEsperados = [novoTeste, novoProvao];

        repositorioTesteORM.CadastrarMultiplosRegistros(registrosEsperados);

        dbContext.SaveChanges();

        List<Teste> testesExistentesOrganizados = repositorioTesteORM.SelecionarRegistros();
        List<Teste> novosTestesOrganizados = registrosEsperados.OrderBy(t => t.DataCriacao).ToList();

        Assert.AreEqual(novosTestesOrganizados.Count, testesExistentesOrganizados.Count);
        CollectionAssert.AreEquivalent(novosTestesOrganizados, testesExistentesOrganizados);
    }

    private List<Questao> CriarQuestoesComMateria(int quantidade, Materia materia)
    {
        List<Questao> questoesX = Builder<Questao>
           .CreateListOfSize(quantidade)
           .All()
           .WithFactory(i =>
           {
               Questao questao = new($"Quest�o {materia.Nome} {i + 1}", materia);
               Alternativa alternativa = new($"Alternativa {i + 1}", questao);
               questao.AderirAlternativa(alternativa);
               return questao;
           })
           .Persist()
           .ToList();

        dbContext.SaveChanges();

        return questoesX;
    }
}
