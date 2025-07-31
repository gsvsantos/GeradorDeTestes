namespace GeradorDeTestes.Testes.Integracao.Compartilhado;

[TestClass]
public static class TestAssemblySetup
{
    public static TesteDbContextFactory Factory { get; private set; }

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext _)
    {
        Factory = new TesteDbContextFactory();

        await Factory.InicializarAsync();
    }

    [AssemblyCleanup]
    public static async Task AssemblyClean()
    {
        await Factory.EncerrarAsync();
    }
}
