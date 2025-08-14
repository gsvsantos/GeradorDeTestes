using GeradorDeTestes.Testes.Interface.ModuloDisciplina;
using GeradorDeTestes.Testes.Interface.ModuloMateria;
using GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;

namespace GeradorDeTestes.Testes.Interface;

[TestClass]
[TestCategory("Testes de Interface de Mat�ria")]
public class MateriaInterfaceTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        DisciplinaFormPageObject disciplinaForm = discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        disciplinaForm
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaForm.ContemMateria("Quatro Opera��es"));
    }

    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        DisciplinaFormPageObject disciplinaForm = discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        disciplinaForm
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickEditar();

        materiaForm
            .PreencherNome("Quatro Opera��es Editada")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaForm.ContemMateria("Quatro Opera��es Editada"));
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        DisciplinaFormPageObject disciplinaForm = discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        disciplinaForm
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickExcluir();

        materiaForm
            .ClickSubmitExcluir("Quatro Opera��es");

        // Assert
        Assert.IsFalse(materiaForm.ContemMateria("Quatro Opera��es"));

    }
}
