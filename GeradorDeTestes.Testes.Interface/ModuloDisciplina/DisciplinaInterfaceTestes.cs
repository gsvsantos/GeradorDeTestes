using GeradorDeTestes.Testes.Interface.ModuloDisciplina;
using GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;

namespace GeradorDeTestes.Testes.InterfaceE2E.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de Interface de Disciplina")]
public sealed class DisciplinaInterfaceTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        discipinaIndex
            .IrPara(enderecoBase);

        // Act
        discipinaIndex
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(discipinaIndex.ContemDisciplina("Matemática"));
    }

    [TestMethod]
    public void Deve_Editar_Disciplina_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .ClickSubmit();

        // Act
        discipinaIndex
            .IrPara(enderecoBase)
            .ClickEditar()
            .PreencherNome("Matemática Editada")
            .ClickSubmit();

        // Assert

        Assert.IsTrue(discipinaIndex.ContemDisciplina("Matemática Editada"));
    }

    [TestMethod]
    public void Deve_Excluir_Disciplina_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .ClickSubmit();

        // Act
        discipinaIndex
            .ClickExcluir()
            .ClickSubmit();

        // Assert

        Assert.IsFalse(discipinaIndex.ContemDisciplina("Matemática"));
    }
}
