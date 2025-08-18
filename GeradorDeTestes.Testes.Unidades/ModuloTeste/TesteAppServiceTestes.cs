using FizzWare.NBuilder;
using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloTeste;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeradorDeTestes.Testes.Unidades;

[TestClass]
[TestCategory("Testes de Unidade de TesteAppService")]
public class TesteAppServiceTestes
{
    private TesteAppService testeAppService;
    private static Disciplina disciplinaPadrao = Builder<Disciplina>.CreateNew()
        .With(d => d.Id = Guid.NewGuid())
        .With(d => d.Nome = "Matemática")
        .Build();
    private List<Materia> materiasPadrao = Builder<Materia>.CreateListOfSize(3)
            .All()
                .With(m => m.Id = Guid.NewGuid())
                .With(m => m.Disciplina = disciplinaPadrao)
                .With(m => m.Serie = EnumSerie.QuartoAnoFundamental)
                .Do(m =>
                {
                    m.Questoes = Builder<Questao>.CreateListOfSize(4)
                        .All()
                            .With(q => q.Id = Guid.NewGuid())
                            .With(q => q.Enunciado = Guid.NewGuid().ToString())
                            .With(q => q.Materia = m)
                            .With(q => q.Finalizado = true)
                        .Build().ToList();
                })
            .Build().ToList();

    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<IRepositorioMateria> repositorioMateriaMock;
    private Mock<IRepositorioQuestao> repositorioQuestaoMock;
    private Mock<IRepositorioTeste> repositorioTesteMock;
    private Mock<ILogger<TesteAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositorioMateriaMock = new Mock<IRepositorioMateria>();
        repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        repositorioTesteMock = new Mock<IRepositorioTeste>();
        loggerMock = new Mock<ILogger<TesteAppService>>();

        testeAppService = new TesteAppService(
            unitOfWorkMock.Object,
            repositorioMateriaMock.Object,
            repositorioQuestaoMock.Object,
            repositorioTesteMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Cadastrar_Teste_Deve_Retornar_Sucesso()
    {
        Teste novoTeste = Builder<Teste>.CreateNew()
            .With(t => t.Disciplina = disciplinaPadrao)
            .With(t => t.Serie = EnumSerie.QuartoAnoFundamental)
            .Build();

        repositorioMateriaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(materiasPadrao);

        repositorioTesteMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Teste>());

        Result resultadoCadastro = testeAppService.CadastrarRegistro(novoTeste);

        repositorioTesteMock.Verify(r => r.CadastrarRegistro(novoTeste), Times.Once());
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once());

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    // cadastrar teste duplicado retorna erro

    // cadastrar teste com excecao lancada retorna erro

    [TestMethod]
    public void Gerar_Questoes_Para_Teste_Deve_Preencher_Questoese_e_Retornar_Sucesso()
    {
        Materia materia1 = materiasPadrao[0];
        Materia materia2 = materiasPadrao[1];

        Teste testeSelecionado = Builder<Teste>.CreateNew()
            .With(t => t.Id = Guid.NewGuid())
            .With(t => t.Disciplina = disciplinaPadrao)
            .With(t => t.Serie = EnumSerie.QuartoAnoFundamental)
            .With(t => t.EhProvao = false)
            .With(t => t.QuantidadeQuestoes = 3)
            .With(t => t.Materias = new List<Materia> { materia1, materia2 })
            .With(t => t.Questoes = new List<Questao>())
            .With(t => t.QuantidadesPorMateria = new List<TesteMateriaQuantidade>
            {
                new TesteMateriaQuantidade
                {
                    Id = Guid.NewGuid(),
                    Materia = materia1,
                    QuantidadeQuestoes = 2
                },
                new TesteMateriaQuantidade
                {
                    Id = Guid.NewGuid(),
                    Materia = materia2,
                    QuantidadeQuestoes = 1
                }
            })
            .Build();

        repositorioQuestaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(materiasPadrao.SelectMany(m => m.Questoes).ToList());

        repositorioTesteMock
            .Setup(r => r.SelecionarRegistroPorId(testeSelecionado.Id))
            .Returns(testeSelecionado);

        Result resultadoGeracao = testeAppService.GerarQuestoesParaTeste(testeSelecionado.Id);

        int countMat1 = testeSelecionado.Questoes.Count(q => q.Materia.Id == materia1.Id);
        int countMat2 = testeSelecionado.Questoes.Count(q => q.Materia.Id == materia2.Id);

        repositorioTesteMock.Verify(r => r.AtualizarRegistro(testeSelecionado), Times.Once());
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once());

        Assert.IsNotNull(resultadoGeracao);
        Assert.IsTrue(resultadoGeracao.IsSuccess);
        Assert.AreEqual(2, countMat1, "Deveria ter 2 questões da matéria 1.");
        Assert.AreEqual(1, countMat2, "Deveria ter 1 questão da matéria 2.");
    }
}
