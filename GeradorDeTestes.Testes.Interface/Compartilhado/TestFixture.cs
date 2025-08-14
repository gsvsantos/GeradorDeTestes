using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected GeradorDeTestesDbContext dbContext;
    protected IWebDriver driver;
    protected string enderecoBase = "https://localhost:7194";
    private string connectionString = "Host=localhost;Port=5432;Database=GeradorDeTestesDb;Username=postgres;Password=SenhaSuperSecreta;";

    [TestInitialize]
    public void ConfigurarTestes()
    {
        dbContext = TesteDbContextFactory.CriarDbContext(connectionString);
        ConfigurarTabelas(dbContext);

        driver = InicializarWebDriver();
    }

    [TestCleanup]
    public void EncerrarTestes()
    {
        EncerrarWebDriver();
    }

    private IWebDriver InicializarWebDriver()
    {
        return ConfigurarChrome();
    }

    private void EncerrarWebDriver()
    {
        driver.Quit();
        driver.Dispose();
    }

    private void ConfigurarTabelas(GeradorDeTestesDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Testes.RemoveRange(dbContext.Testes);
        dbContext.Questoes.RemoveRange(dbContext.Questoes);
        dbContext.Materias.RemoveRange(dbContext.Materias);
        dbContext.Disciplinas.RemoveRange(dbContext.Disciplinas);

        dbContext.SaveChanges();
    }

    private static IWebDriver ConfigurarChrome()
    {
        bool esconderChrome = true;

        ChromeOptions options = new();

        if (esconderChrome)
            options.AddArgument("--headless=new");

        return new ChromeDriver(options);
    }
}
