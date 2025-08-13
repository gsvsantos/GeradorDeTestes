using FizzWare.NBuilder;
using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloQuestao;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de QuestaoAppService")]
public class QuestaoAppServiceTestes
{
    private QuestaoAppService questaoAppService;
    private Materia materiaPadrao = new("Subtração", new("Matemática"), EnumSerie.QuartoAnoFundamental);


    private Mock<IGeradorQuestoes> geradorQuestoesMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<IRepositorioQuestao> repositorioQuestaoMock;
    private Mock<IRepositorioTeste> repositorioTesteMock;
    private Mock<ILogger<QuestaoAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        geradorQuestoesMock = new Mock<IGeradorQuestoes>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        repositorioTesteMock = new Mock<IRepositorioTeste>();
        loggerMock = new Mock<ILogger<QuestaoAppService>>();

        questaoAppService = new QuestaoAppService(
            geradorQuestoesMock.Object,
            unitOfWorkMock.Object,
            repositorioQuestaoMock.Object,
            repositorioTesteMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_Questao_Deve_Retornar_Sucesso()
    {
        Questao novaQuestao = new("Quanto é 53 - 38?", materiaPadrao);

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao>());

        Result resultadoCadastro = questaoAppService.CadastrarRegistro(novaQuestao);

        repositorioQuestaoMock.Verify(r => r.CadastrarRegistro(novaQuestao), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Questao_Duplicada_Deve_Retornar_Falha()
    {
        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao> { new("Quanto é 53 - 38?", materiaPadrao) });

        Questao novaQuestao = new("Quanto é 53 - 38?", materiaPadrao);

        Result resultadoCadastro = questaoAppService.CadastrarRegistro(novaQuestao);

        repositorioQuestaoMock.Verify(r => r.CadastrarRegistro(novaQuestao), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoCadastro.Errors[0].Message);
    }

    [TestMethod]
    public void Cadastrar_Questao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        Questao novaQuestao = new("Quanto é 53 - 38?", materiaPadrao);

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao>());

        repositorioQuestaoMock
            .Setup(r => r.CadastrarRegistro(novaQuestao))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro no cadastro."));

        Result resultadoCadastro = questaoAppService.CadastrarRegistro(novaQuestao);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
    }

    [TestMethod]
    public void Editar_Questao_Deve_Retornar_Sucesso()
    {
        Questao novaQuestao = new("Quanto é 53 + 38?", materiaPadrao);

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao> { novaQuestao });

        Questao questaoEditada = new("Quanto é 53 - 38?", materiaPadrao);

        Result resultadoEdicao = questaoAppService.EditarRegistro(novaQuestao.Id, questaoEditada);

        repositorioQuestaoMock.Verify(r => r.EditarRegistro(novaQuestao.Id, questaoEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Questao_Duplicada_Deve_Retornar_Falha()
    {
        Questao novaQuestao = new("Quanto é 53 + 38?", materiaPadrao) { Id = Guid.NewGuid() };

        List<Questao> questoesExistentes = new()
        {
            novaQuestao,
            new("Quanto é 53 - 38?", materiaPadrao)
        };

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(questoesExistentes);

        Questao questaoEditada = new("Quanto é 53 - 38?", materiaPadrao);

        Result resultadoEdicao = questaoAppService.EditarRegistro(novaQuestao.Id, questaoEditada);

        repositorioQuestaoMock.Verify(r => r.EditarRegistro(novaQuestao.Id, questaoEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoEdicao.Errors[0].Message);
    }

    [TestMethod]
    public void Editar_Questao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        Questao novaQuestao = new("Quanto é 53 + 38?", materiaPadrao) { Id = Guid.NewGuid() };

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao>() { novaQuestao });

        Questao questaoEditada = new("Quanto é 53 - 38?", materiaPadrao);

        // Simula exceção na persistência.
        repositorioQuestaoMock
            .Setup(r => r.EditarRegistro(novaQuestao.Id, questaoEditada))
            .Throws(new Exception("Erro inesperado"));

        // E/ou exceção no commit.
        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na edição."));

        Result resultadoEdicao = questaoAppService.EditarRegistro(novaQuestao.Id, questaoEditada);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
    }

    [TestMethod]
    public void Excluir_Questao_Sem_Relacao_Deve_Retornar_Sucesso()
    {
        Questao novaQuestao = Builder<Questao>.CreateNew().Build();

        repositorioQuestaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao>());

        repositorioTesteMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Teste>());

        Result resultadoExclusao = questaoAppService.ExcluirRegistro(novaQuestao.Id);

        repositorioQuestaoMock.Verify(r => r.ExcluirRegistro(novaQuestao.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Questao_Com_Relacao_Deve_Retornar_Falha()
    {
        Questao novaQuestao = Builder<Questao>.CreateNew().Build();
        Teste novoTeste = Builder<Teste>.CreateNew().With(t => t.Questoes = new() { novaQuestao }).Build();

        repositorioQuestaoMock.Setup(r => r.SelecionarRegistroPorId(novaQuestao.Id))
            .Returns(novaQuestao);

        repositorioTesteMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Teste> { novoTeste });

        Result resultadoExclusao = questaoAppService.ExcluirRegistro(novaQuestao.Id);

        repositorioQuestaoMock.Verify(r => r.ExcluirRegistro(novaQuestao.Id), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro Vinculado", mensagemErro);
    }
}
