using GeradorDeTestes.Testes.Interface.Compartilhado;
using GeradorDeTestes.Testes.Interface.ModuloDisciplina;

namespace GeradorDeTestes.Testes.Interface.ModuloMateria;

[TestClass]
[TestCategory("Testes de Interface de Matéria")]
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

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver);

        materiaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Quatro Operações")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
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

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver);

        materiaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Quatro Operações")
            .SelecionarDisciplina("Matemática")
            .SelecionarSerie("4º ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        MateriaFormPageObject materiaForm = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase)
            .ClickExcluir();

        materiaForm
            .ClickSubmitExcluir("Quatro Operações");

        // Assert
        Assert.IsFalse(materiaForm.ContemMateria("Quatro Operações"));

    }
}
