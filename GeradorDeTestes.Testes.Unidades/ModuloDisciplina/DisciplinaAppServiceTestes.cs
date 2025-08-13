using FizzWare.NBuilder;
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
[TestCategory("Testes de Unidade de DisciplinaAppService")]
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
        // Arrange - n�o existe disciplinas existentes com o mesmo nome.
        Disciplina novaDisciplina = new("Matem�tica");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina>());

        // Act - executa o contrato p�blico do servi�o.
        Result resultadoCadastro = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        // Assert � efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.CadastrarRegistro(novaDisciplina), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Disciplina_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange � j� existe disciplina com o mesmo nome.
        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina> { new("Matem�tica") });

        Disciplina novaDisciplina = new("Matem�tica");

        // Act
        Result resultadoCadastro = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        // Assert � efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.CadastrarRegistro(It.IsAny<Disciplina>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoCadastro.Errors[0].Message);
    }

    [TestMethod]
    public void Cadastrar_Disciplina_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange - cen�rio feliz at� tentar persistir/commitar.
        Disciplina novaDisciplina = new("Matem�tica");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina>());

        // Simula exce��o na persist�ncia.
        repositorioDisciplinaMock?
            .Setup(r => r.CadastrarRegistro(novaDisciplina))
            .Throws(new Exception("Erro inesperado"));

        // E/ou exce��o no commit.
        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro no cadastro."));

        // Act
        Result resultadoCadastro = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        // Assert � efeitos e contrato.
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once());

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
    }

    [TestMethod]
    public void Editar_Disciplina_Deve_Retornar_Sucesso()
    {
        // Arrange � existe uma disciplina previamente cadastrada.
        Disciplina novaDisciplina = new("mate mat�ca");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina> { novaDisciplina });

        // Nova vers�o (editar nome).
        Disciplina disciplinaEditada = new("Matem�tica");

        // Act
        Result resultadoEdicao = disciplinaAppService.EditarRegistro(novaDisciplina.Id, disciplinaEditada);

        // Assert � efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.EditarRegistro(novaDisciplina.Id, disciplinaEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Disciplina_Duplicada_Deve_Retornar_Falha()
    {
        Disciplina novaDisciplina = new("mate mat�ca") { Id = Guid.NewGuid() };

        // Arrange � existe duas disciplinas previamente cadastradas, uma ter� o mesmo nome da editada.
        List<Disciplina> disciplinasExistentes = new()
        {
            novaDisciplina,
            new("Matem�tica")
        };

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(disciplinasExistentes);

        // Nova vers�o para tentar editar o nome do alvo.
        Disciplina disciplinaEditada = new("Matem�tica");

        // Act
        Result resultadoEdicao = disciplinaAppService.EditarRegistro(novaDisciplina.Id, disciplinaEditada);

        // Assert � efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.EditarRegistro(novaDisciplina.Id, disciplinaEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro Duplicado", resultadoEdicao.Errors[0].Message);
    }

    [TestMethod]
    public void Editar_Disciplina_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange - cen�rio feliz at� tentar persistir/commitar.
        Disciplina novaDisciplina = new("mate mat�ca");

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Disciplina> { novaDisciplina });

        // Nova vers�o (editar nome).
        Disciplina disciplinaEditada = new("Matem�tica");

        // Simula exce��o na persist�ncia.
        repositorioDisciplinaMock?
            .Setup(r => r.EditarRegistro(novaDisciplina.Id, disciplinaEditada))
            .Throws(new Exception("Erro inesperado"));

        // E/ou exce��o no commit.
        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na edi��o."));

        // Act
        Result resultadoEdicao = disciplinaAppService.EditarRegistro(novaDisciplina.Id, disciplinaEditada);

        // Assert - efeito e contrato.
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once());

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.AreEqual("Ocorreu um erro interno no servidor.", mensagemErro);
        Assert.IsTrue(resultadoEdicao.IsFailed);
    }

    [TestMethod]
    public void Excluir_Disciplina_Sem_Relacao_Deve_Retornar_Sucesso()
    {
        // Arrange - cen�rio feliz at� o final, disciplina sem rela��es.
        Disciplina novaDisciplina = Builder<Disciplina>.CreateNew().Build();

        repositorioMateriaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        repositorioTesteMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Teste>());

        // Act
        Result resultadoExclusao = disciplinaAppService.ExcluirRegistro(novaDisciplina.Id);

        // Assert - efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.ExcluirRegistro(novaDisciplina.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Disciplina_Com_Relacao_Deve_Retornar_Falha()
    {
        // Arrange - disciplina cont�m duas rela��es.
        Disciplina novaDisciplina = Builder<Disciplina>.CreateNew().Build();

        Materia novaMateria = Builder<Materia>.CreateNew().With(m => m.Disciplina = novaDisciplina).Build();
        Teste novoTeste = Builder<Teste>.CreateNew().With(t => t.Disciplina = novaDisciplina).Build();

        repositorioDisciplinaMock
            .Setup(r => r.SelecionarRegistroPorId(novaDisciplina.Id))
            .Returns(novaDisciplina);

        repositorioMateriaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia> { novaMateria });

        repositorioTesteMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Teste> { novoTeste });

        // Act
        Result resultadoExclusao = disciplinaAppService.ExcluirRegistro(novaDisciplina.Id);

        // Assert - efeitos e contrato.
        repositorioDisciplinaMock.Verify(r => r.ExcluirRegistro(It.IsAny<Guid>()), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro Vinculado", mensagemErro);
    }
}
