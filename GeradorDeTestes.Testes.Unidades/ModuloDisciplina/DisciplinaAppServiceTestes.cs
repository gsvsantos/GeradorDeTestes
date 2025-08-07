using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de Disciplina")]
public class DisciplinaAppServiceTestes
{
    private DisciplinaAppService disciplinaAppService;

    private Mock<IGeradorDisciplinas> geradorDisciplinasMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<IRepositorioDisciplina> repositorioDisciplinaMock;
    private Mock<IRepositorioMateria> repositorioMateriaMock;
    private Mock<IRepositorioTeste> repositorioTesteMock;
    private Mock<ILogger<DisciplinaAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        geradorDisciplinasMock = new Mock<IGeradorDisciplinas>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositorioDisciplinaMock = new Mock<IRepositorioDisciplina>();
        repositorioMateriaMock = new Mock<IRepositorioMateria>();
        repositorioTesteMock = new Mock<IRepositorioTeste>();
        loggerMock = new Mock<ILogger<DisciplinaAppService>>();

        disciplinaAppService = new DisciplinaAppService(
            geradorDisciplinasMock.Object,
            unitOfWorkMock.Object,
            repositorioDisciplinaMock.Object,
            repositorioMateriaMock.Object,
            repositorioTesteMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_Disciplina_Deve_Retornar_Sucesso()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matemática");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina>());

        // Act
        Result resultado = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        // Assert
        repositorioDisciplinaMock.Verify(r => r.CadastrarRegistro(novaDisciplina), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Disciplina_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matemática");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina> { new("Matemática") });

        // Act
        Result resultado = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        // Assert
        repositorioDisciplinaMock.Verify(r => r.CadastrarRegistro(It.IsAny<Disciplina>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultado.Errors[0].Message);
    }

    [TestMethod]
    public void Cadastrar_Disciplina_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        Disciplina novaDisciplina = new("Matemática");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina>());

        repositorioDisciplinaMock?
            .Setup(r => r.CadastrarRegistro(novaDisciplina))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro no cadastro."));

        Result resultado = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(resultado);

        string mensagemErro = resultado.Errors[0].Message;

        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);

        Assert.IsTrue(resultado.IsFailed);
    }
}
