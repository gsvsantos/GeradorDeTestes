using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
using GeradorDeTestes.Testes.Integracao.Compartilhado;

namespace GeradorDeTestes.Testes.Integracao;

[TestClass]
public abstract class TestFixture
{
    private static TesteDbContextFactory? factory;
    protected GeradorDeTestesDbContext dbContext;

    protected RepositorioDisciplinaORM repositorioDisciplinaORM;
    protected RepositorioMateriaORM repositorioMateriaORM;
    protected RepositorioQuestaoORM repositorioQuestaoORM;
    protected RepositorioTesteORM repositorioTesteORM;


    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        factory = new TesteDbContextFactory();

        await factory.InicializarAsync();
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        if (factory is not null)
            await factory.EncerrarAsync();
    }

    [TestInitialize]
    public virtual void ConfigurarTestes()
    {
        dbContext = factory!.CriarDbContext();

        if (dbContext is null)
            throw new ArgumentNullException("TesteDbContextFactory não foi inicializado corretamente.");

        ConfigurarTabelas(dbContext);

        repositorioDisciplinaORM = new RepositorioDisciplinaORM(dbContext);
        repositorioQuestaoORM = new RepositorioQuestaoORM(dbContext);
        repositorioMateriaORM = new RepositorioMateriaORM(dbContext);
        repositorioTesteORM = new RepositorioTesteORM(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Disciplina>(repositorioDisciplinaORM.CadastrarRegistro);
        BuilderSetup.SetCreatePersistenceMethod<IList<Disciplina>>(repositorioDisciplinaORM.CadastrarMultiplosRegistros);
        BuilderSetup.SetCreatePersistenceMethod<Materia>(repositorioMateriaORM.CadastrarRegistro);
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
