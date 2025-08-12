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

        discipinaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        // Act
        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver)
            .IrPara(enderecoBase);

        materiaIndex
            .ClickCadastrar()
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaIndex.ContemMateria("Quatro Opera��es"));
    }

    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        new DisciplinaIndexPageObject(driver!)
           .IrPara(enderecoBase)
           .ClickCadastrar()
           .PreencherNome("Matem�tica")
           .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver!)
            .IrPara(enderecoBase);

        materiaIndex
            .ClickCadastrar()
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaIndex
            .ClickEditar()
            .PreencherNome("Quatro Opera��es Editada")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(materiaIndex.ContemMateria("Quatro Opera��es Editada"));
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        new DisciplinaIndexPageObject(driver!)
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNome("Matem�tica")
            .ClickSubmit();

        MateriaIndexPageObject materiaIndex = new MateriaIndexPageObject(driver!)
            .IrPara(enderecoBase);

        materiaIndex
            .ClickCadastrar()
            .PreencherNome("Quatro Opera��es")
            .SelecionarDisciplina("Matem�tica")
            .SelecionarSerie("4� ano do Ensino Fundamental")
            .ClickSubmit();

        // Act
        materiaIndex
            .ClickExcluir()
            .ClickSubmitExcluir();

        // Assert
        Assert.IsFalse(materiaIndex.ContemMateria("Quatro Opera��es"));

    }
}
