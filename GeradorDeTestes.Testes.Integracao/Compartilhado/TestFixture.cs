using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
using Testcontainers.PostgreSql;

namespace GeradorDeTestes.Testes.Integracao;

[TestClass]
public abstract class TestFixture
{
    protected GeradorDeTestesDbContext dbContext;

    protected RepositorioDisciplinaORM repositorioDisciplinaORM;
    protected RepositorioMateriaORM repositorioMateriaORM;
    protected RepositorioQuestaoORM repositorioQuestaoORM;
    protected RepositorioTesteORM repositorioTesteORM;

    private static IDatabaseContainer? container;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("gerador-de-testes-testesdb")
            .WithDatabase("GeradorDeTestesDbTestes")
            .WithUsername("postgres")
            .WithPassword("SenhaSuperSecreta")
            .WithPortBinding(5434, 5432)
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync();
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public virtual void ConfigurarTestes()
    {
        if (container is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado corretamente.");

        dbContext = TesteDbContextFactory.CriarDbContext(container.GetConnectionString());

        ConfigurarTabelas(dbContext);

        repositorioDisciplinaORM = new RepositorioDisciplinaORM(dbContext);
        repositorioQuestaoORM = new RepositorioQuestaoORM(dbContext);
        repositorioMateriaORM = new RepositorioMateriaORM(dbContext);
        repositorioTesteORM = new RepositorioTesteORM(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Disciplina>(repositorioDisciplinaORM.CadastrarRegistro);
        BuilderSetup.SetCreatePersistenceMethod<IList<Disciplina>>(repositorioDisciplinaORM.CadastrarMultiplosRegistros);

        BuilderSetup.SetCreatePersistenceMethod<Materia>(repositorioMateriaORM.CadastrarRegistro);
        BuilderSetup.SetCreatePersistenceMethod<IList<Materia>>(repositorioMateriaORM.CadastrarMultiplosRegistros);

        BuilderSetup.SetCreatePersistenceMethod<Questao>(repositorioQuestaoORM.CadastrarRegistro);
        BuilderSetup.SetCreatePersistenceMethod<IList<Questao>>(repositorioQuestaoORM.CadastrarMultiplosRegistros);

        BuilderSetup.SetCreatePersistenceMethod<Teste>(repositorioTesteORM.CadastrarRegistro);
        BuilderSetup.SetCreatePersistenceMethod<IList<Teste>>(repositorioTesteORM.CadastrarMultiplosRegistros);
    }

    private static async Task InicializarBancoDadosAsync()
    {
        await container!.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        await container!.StopAsync();
        await container.DisposeAsync();
    }

    private static void ConfigurarTabelas(GeradorDeTestesDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Testes.RemoveRange(dbContext.Testes);
        dbContext.Questoes.RemoveRange(dbContext.Questoes);
        dbContext.Materias.RemoveRange(dbContext.Materias);
        dbContext.Disciplinas.RemoveRange(dbContext.Disciplinas);

        dbContext.SaveChanges();
    }
}
