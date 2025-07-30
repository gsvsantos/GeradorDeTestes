using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Testes.Integracao.Compartilhado;

namespace GeradorDeTestes.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integração de Disciplina")]
public sealed class RepositorioDisciplinaORMTestes
{
    private GeradorDeTestesDbContext dbContext;
    private RepositorioDisciplinaORM repositorioDisciplinaORM;

    #region Padrão AAA
    // Arrange - Arranjo
    // Act - Ação
    // Assert - Asseção 
    #endregion

    [TestInitialize]
    public void ConfigurarTestes()
    {
        dbContext = TesteDbContextFactory.CriarDbContext();

        repositorioDisciplinaORM = new RepositorioDisciplinaORM(dbContext);
    }

    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matemática");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);
        dbContext.SaveChanges();

        // Act
        Disciplina disciplinaSelecionada = repositorioDisciplinaORM.SelecionarRegistroPorId(novaDisciplina.Id)!;

        // Assert
        Assert.IsNotNull(disciplinaSelecionada, "Não conseguiu selecionar a disciplina.");
        Assert.AreEqual(novaDisciplina.Id, disciplinaSelecionada.Id, "A disciplina selecionada não condiz com a disciplina cadastrada.");
        Assert.AreEqual(novaDisciplina.Nome, disciplinaSelecionada.Nome, "A disciplina selecionada não condiz com a disciplina cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Disciplina_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matematica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);
        dbContext.SaveChanges();

        Disciplina disciplinaEditada = new("Matemática");

        // Act
        bool conseguiuEditar = repositorioDisciplinaORM.EditarRegistro(novaDisciplina.Id, disciplinaEditada);
        dbContext.SaveChanges();

        // Assert
        Disciplina disciplinaSelecionada = repositorioDisciplinaORM.SelecionarRegistroPorId(novaDisciplina.Id)!;

        Assert.IsNotNull(disciplinaSelecionada, "Não conseguiu selecionar a disciplina.");
        Assert.AreEqual(novaDisciplina.Id, disciplinaSelecionada.Id, "A disciplina selecionada não condiz com a disciplina editada.");
        Assert.AreEqual(disciplinaEditada.Nome, disciplinaSelecionada.Nome, "A disciplina selecionada não condiz com a disciplina editada.");
        Assert.IsTrue(conseguiuEditar, "Não conseguiu editar a disciplina.");
    }

    [TestMethod]
    public void Deve_Excluir_Disciplina_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matemática");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);
        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = repositorioDisciplinaORM.ExcluirRegistro(novaDisciplina.Id);
        dbContext.SaveChanges();

        // Assert
        Disciplina? disciplinaSelecionada = repositorioDisciplinaORM.SelecionarRegistroPorId(novaDisciplina.Id);

        Assert.IsNull(disciplinaSelecionada, "Não conseguiu selecionar a disciplina.");
        Assert.IsTrue(conseguiuExcluir, "Não conseguiu excluir a disciplina.");
    }

    [TestMethod]
    public void Deve_Selecionar_Disciplina_Corretamente()
    {
        // Arrange
        List<Disciplina> novasDisciplinas = new()
        {
            new Disciplina("Matemática"),
            new Disciplina("Português"),
            new Disciplina("Inglês")
        };

        foreach (Disciplina disciplina in novasDisciplinas)
        {
            repositorioDisciplinaORM.CadastrarRegistro(disciplina);
        }

        dbContext.SaveChanges();

        // Act
        List<Disciplina> disciplinasExistentes = repositorioDisciplinaORM.SelecionarRegistros();

        // Assert
        CollectionAssert.AreEqual(
            novasDisciplinas.Select(d => d.Nome)
            .OrderBy(n => n).ToList(),
            disciplinasExistentes.Select(d => d.Nome)
            .OrderBy(n => n).ToList(),
            "Disciplinas selecionadas não condiz com as disciplinas cadastradas."
            );
    }
}
