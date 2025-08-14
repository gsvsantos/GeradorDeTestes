using GeradorDeTestes.Testes.Interface.ModuloDisciplina;
using GeradorDeTestes.Testes.Interface.ModuloMateria;
using GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;

namespace GeradorDeTestes.Testes.Interface;

[TestClass]
[TestCategory("Testes de Interface de Matéria")]
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
            .PreencherNome("Matemática")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Operações")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaForm.ContemMateria("Quatro Operações"));
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
            .PreencherNome("Matemática")
            .ClickSubmit();

        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Operações")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickEditar();

        materiaForm
            .PreencherNome("Quatro Operações Editada")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaForm.ContemMateria("Quatro Operações Editada"));
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
            .PreencherNome("Matemática")
            .ClickSubmit();

        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        materiaForm
            .PreencherNome("Quatro Operações")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickExcluir();

        materiaForm
            .ClickSubmitExcluir("Quatro Operações");

        // Assert
        Assert.IsFalse(materiaForm.ContemMateria("Quatro Operações"));

    }
}
