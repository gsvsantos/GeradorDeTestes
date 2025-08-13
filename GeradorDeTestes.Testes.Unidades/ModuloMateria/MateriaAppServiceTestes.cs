using FizzWare.NBuilder;
using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de MatériaAppService")]
public class MateriaAppServiceTestes
{
    private MateriaAppService materiaAppService;
    private Disciplina disciplinaPadrao = new("Matemática");

    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<IRepositorioMateria> repositorioMateriaMock;
    private Mock<IRepositorioQuestao> repositorioQuestaoMock;
    private Mock<ILogger<MateriaAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositorioMateriaMock = new Mock<IRepositorioMateria>();
        repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        loggerMock = new Mock<ILogger<MateriaAppService>>();

        materiaAppService = new MateriaAppService(
            unitOfWorkMock.Object,
            repositorioMateriaMock.Object,
            repositorioQuestaoMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_Materia_Deve_Retornar_Sucesso()
    {
        Materia novaMateria = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        Result resultadoCadastro = materiaAppService.CadastrarRegistro(novaMateria);

        repositorioMateriaMock.Verify(r => r.CadastrarRegistro(novaMateria), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Materia_Duplicada_Deve_Retornar_Falha()
    {
        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia> { new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental) });

        Materia novaMateria = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        Result resultadoCadastro = materiaAppService.CadastrarRegistro(novaMateria);

        repositorioMateriaMock.Verify(r => r.CadastrarRegistro(It.IsAny<Materia>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoCadastro.Errors[0].Message);
    }

    [TestMethod]
    public void Cadastrar_Materia_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        Materia novaMateria = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        repositorioMateriaMock
            .Setup(r => r.CadastrarRegistro(novaMateria))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro no cadastro."));

        Result resultadoCadastro = materiaAppService.CadastrarRegistro(novaMateria);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
    }

    [TestMethod]
    public void Editar_Materia_Deve_Retornar_Sucesso()
    {
        Materia novaMateria = new("Soma", disciplinaPadrao, EnumSerie.QuintoAnoFundamental);

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        Materia materiaEditada = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        Result resultadoEdicao = materiaAppService.EditarRegistro(novaMateria.Id, materiaEditada);

        repositorioMateriaMock.Verify(r => r.EditarRegistro(novaMateria.Id, materiaEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Materia_Duplicada_Deve_Retornar_Falha()
    {
        Materia novaMateria = new("Soma", disciplinaPadrao, EnumSerie.QuintoAnoFundamental) { Id = Guid.NewGuid() };

        List<Materia> materiasExistentes = new()
        {
            novaMateria,
            new("Fração", disciplinaPadrao, EnumSerie.SextoAnoFundamental)
        };

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(materiasExistentes);

        Materia materiaEditada = new("Fração", disciplinaPadrao, EnumSerie.SextoAnoFundamental);

        Result resultadoEdicao = materiaAppService.EditarRegistro(novaMateria.Id, materiaEditada);

        repositorioMateriaMock.Verify(r => r.EditarRegistro(novaMateria.Id, materiaEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoEdicao.Errors[0].Message);
    }

    [TestMethod]
    public void Editar_Materia_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        Materia novaMateria = new("Soma", disciplinaPadrao, EnumSerie.QuintoAnoFundamental);

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        Materia materiaEditada = new("Subtração", disciplinaPadrao, EnumSerie.QuartoAnoFundamental);

        repositorioMateriaMock?
            .Setup(r => r.EditarRegistro(novaMateria.Id, materiaEditada))
            .Throws(new Exception("Erro inesperado"));

        // E/ou exceção no commit.
        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na edição."));

        // Act
        Result resultadoEdicao = materiaAppService.EditarRegistro(novaMateria.Id, materiaEditada);

        // Assert - efeito e contrato.
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
        Assert.IsTrue(resultadoEdicao.IsFailed);
    }

    [TestMethod]
    public void Excluir_Materia_Sem_Relacao_Deve_Retornar_Sucesso()
    {
        Materia novaMateria = Builder<Materia>.CreateNew().Build();

        repositorioMateriaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        repositorioQuestaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao>());

        Result resultadoExclusao = materiaAppService.ExcluirRegistro(novaMateria.Id);

        repositorioMateriaMock.Verify(r => r.ExcluirRegistro(novaMateria.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Materia_Com_Relacao_Deve_Retornar_Falha()
    {
        // Arrange - cenário feliz até o final, disciplina sem relações.
        Materia novaMateria = Builder<Materia>.CreateNew().Build();
        Questao novaQuestao = Builder<Questao>.CreateNew().With(q => q.Materia = novaMateria).Build();

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistroPorId(novaMateria.Id))
            .Returns(novaMateria);

        repositorioQuestaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Questao> { novaQuestao });

        // Act
        Result resultadoExclusao = materiaAppService.ExcluirRegistro(novaMateria.Id);

        // Assert - efeitos e contrato.
        repositorioMateriaMock.Verify(r => r.ExcluirRegistro(novaMateria.Id), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro Vinculado", mensagemErro);
    }
}
