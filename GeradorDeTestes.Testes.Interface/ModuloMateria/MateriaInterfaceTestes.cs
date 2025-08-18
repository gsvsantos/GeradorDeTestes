using GeradorDeTestes.Testes.Interface.Compartilhado;
using GeradorDeTestes.Testes.Interface.ModuloDisciplina;

namespace GeradorDeTestes.Testes.Interface.ModuloMateria;

[TestClass]
[TestCategory("Testes de Interface de Mat�ria")]
public class MateriaInterfaceTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        // Arrange
        DisciplinaIndexPageObject discipinaIndex = new(driver);

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
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

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver);

        materiaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
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

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver);

        materiaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickExcluir();

        materiaForm
            .ClickSubmitExcluir("Quatro Opera��es");

        // Assert
        Assert.IsFalse(materiaForm.ContemMateria("Quatro Opera��es"));

    }
}
