using GeradorDeTestes.Testes.Interface.Compartilhado;
using GeradorDeTestes.Testes.Interface.ModuloDisciplina;

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
        DisciplinaFormPageObject disciplinaForm = discipinaIndex
            .ClickCadastrar();

        disciplinaForm
            .PreencherNome("Matemática")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(disciplinaForm.ContemDisciplina("Matemática"));
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
        DisciplinaFormPageObject disciplinaForm = new DisciplinaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickEditar();

        disciplinaForm
            .PreencherNome("Matemática Editada")
            .ClickSubmit();

        // Assert

        Assert.IsTrue(disciplinaForm.ContemDisciplina("Matemática Editada"));
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
        DisciplinaFormPageObject disciplinaForm = new DisciplinaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickExcluir();

        disciplinaForm
            .ClickSubmitExcluir("Matemática");

        // Assert

        Assert.IsFalse(disciplinaForm.ContemDisciplina("Matemática"));
    }
}
