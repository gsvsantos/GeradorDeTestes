using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Testcontainers.PostgreSql;

namespace GeradorDeTestes.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static GeradorDeTestesDbContext dbContext;
    protected static IWebDriver driver;
    protected static string enderecoBase;

    private static IDatabaseContainer dbContainer;
    private readonly static int dbPort = 5432;

    private static IContainer appContainer;
    private readonly static int appPort = 8080;

    private static IContainer seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static IConfiguration configuracao;
    private static DotNet.Testcontainers.Networks.INetwork? rede;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

        rede = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync();

        await InicializarAplicacaoAsync();

        await InicializarWebDriverAsync();
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        EncerrarWebDriverAsync();

        await EncerrarAplicacaoAsync();

        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public void InicializarTeste()
    {
        if (dbContainer is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado.");

        dbContext = TesteDbContextFactory.CriarDbContext(dbContainer.GetConnectionString());

        ConfigurarTabelas(dbContext);
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

    private static async Task InicializarBancoDadosAsync()
    {
        dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("gerador-de-testes-db-e2e")
            .WithDatabase("GeradorDeTestesDbTestes")
            .WithUsername("postgres")
            .WithPassword("SenhaSuperSecreta")
            .WithPortBinding(dbPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("gerador-de-testes-db-e2e")
            .WithCleanUp(true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                .UntilPortIsAvailable(dbPort)
            )
            .Build();

        await dbContainer.StartAsync();
    }

    private static async Task InicializarAplicacaoAsync()
    {
        // Configura image via dockerfile
        IFutureDockerImage image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithName("gerador-de-testes-db-e2e:latest")
            .Build();

        await image.CreateAsync().ConfigureAwait(false);

        // Configurar a connectionString para o banco dentro da rede
        string? connectionStringRede = dbContainer.GetConnectionString()
            .Replace(dbContainer.Hostname, "gerador-de-testes-db-e2e")
            .Replace(dbContainer.GetMappedPublicPort(dbPort).ToString(), "5432");

        // Configura container da aplicacao e inicializa enderecoBase
        appContainer = new ContainerBuilder()
            .WithImage(image)
            .WithPortBinding(appPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("gerador-de-testes-webapp")
            .WithName("gerador-de-testes-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", connectionStringRede)
            .WithEnvironment("GEMINI_API_KEY", configuracao["GEMINI_API_KEY"])
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao["NEWRELIC_LICENSE_KEY"])
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                .UntilPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health"))
            )
            .WithCleanUp(true)
            .Build();

        await appContainer.StartAsync();

        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }

    private static async Task InicializarWebDriverAsync()
    {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:nightly")
            .WithPortBinding(seleniumPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("gerador-de-testes-selenium-e2e")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithName("gerador-de-testes-selenium-e2e")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                .UntilPortIsAvailable(seleniumPort)
            )
            .Build();

        await seleniumContainer.StartAsync();

        Uri enderecoSelenium = new($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");

        ChromeOptions options = new();
        //options.AddArgument("--headless=new");

        driver = new RemoteWebDriver(enderecoSelenium, options);
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (dbContainer is not null)
            await dbContainer.DisposeAsync();
    }

    private static async Task EncerrarAplicacaoAsync()
    {
        if (appContainer is not null)
            await appContainer.DisposeAsync();
    }

    private static async Task EncerrarWebDriverAsync()
    {
        driver?.Quit();
        driver?.Dispose();

        if (seleniumContainer is not null)
            await seleniumContainer.DisposeAsync();
    }
}
