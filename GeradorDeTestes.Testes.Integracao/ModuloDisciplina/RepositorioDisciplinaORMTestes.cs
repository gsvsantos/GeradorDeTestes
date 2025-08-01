using GeradorDeTestes.Dominio.ModuloDisciplina;

namespace GeradorDeTestes.Testes.Integracao.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de Integração de Disciplina")]
public sealed class RepositorioDisciplinaORMTestes : TestFixture
{
    #region Padrão AAA
    // Arrange - Arranjo
    // Act - Ação
    // Assert - Asseção 
    #endregion

    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matemática");

        // Act
        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);

        dbContext.SaveChanges();

        // Assert
        Disciplina disciplinaSelecionada = repositorioDisciplinaORM.SelecionarRegistroPorId(novaDisciplina.Id)!;

        Assert.IsNotNull(disciplinaSelecionada, "Não conseguiu selecionar a disciplina.");
        Assert.AreEqual(novaDisciplina, disciplinaSelecionada, "A disciplina selecionada não condiz com a disciplina cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Disciplina_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matematica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);

        dbContext.SaveChanges();

        // Act
        Disciplina disciplinaEditada = new("Matemática");

        bool conseguiuEditar = repositorioDisciplinaORM.EditarRegistro(novaDisciplina.Id, disciplinaEditada);

        dbContext.SaveChanges();

        // Assert
        Disciplina disciplinaSelecionada = repositorioDisciplinaORM.SelecionarRegistroPorId(novaDisciplina.Id)!;

        Assert.IsTrue(conseguiuEditar, "Não conseguiu editar a disciplina.");
        Assert.IsNotNull(disciplinaSelecionada, "Não conseguiu selecionar a disciplina.");
        Assert.AreEqual(novaDisciplina, disciplinaSelecionada, "A disciplina selecionada não condiz com a disciplina editada.");
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

        Assert.IsTrue(conseguiuExcluir, "Não conseguiu excluir a disciplina.");
        Assert.IsNull(disciplinaSelecionada, "A disciplina ainda está no banco após exclusão.");
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_As_Disciplinas_Corretamente()
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
        Assert.AreEqual(novasDisciplinas.Count, disciplinasExistentes.Count);
        CollectionAssert.AreEqual(
            novasDisciplinas.Select(d => d.Nome)
            .OrderBy(n => n).ToList(),
            disciplinasExistentes.Select(d => d.Nome)
            .OrderBy(n => n).ToList(),
            "Disciplinas selecionadas não condiz com as disciplinas cadastradas."
            );
    }
}
