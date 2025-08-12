using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static GeradorDeTestesDbContext dbContext;
    protected static IWebDriver driver;
    protected static string enderecoBase = "https://localhost:7194";
    private static string connectionString = "Host=localhost;Port=5432;Database=GeradorDeTestesDb;Username=postgres;Password=@GStavo02!;";

    [TestInitialize]
    public void ConfigurarTestes()
    {
        dbContext = TesteDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);

        InicializarWebDriver();
    }

    [TestCleanup]
    public void EncerrarTestes()
    {
        EncerrarWebDriver();
    }

    private static void InicializarWebDriver()
    {
        driver = EsconderChrome(false);
    }

    private static void EncerrarWebDriver()
    {
        driver.Quit();
        driver.Dispose();
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

    private static ChromeDriver EsconderChrome(bool value)
    {
        if (value)
        {
            ChromeOptions options = new();
            options.AddArgument("--headless");

            return new ChromeDriver(options);
        }
        else
        {
            return new ChromeDriver();
        }
    }
}
