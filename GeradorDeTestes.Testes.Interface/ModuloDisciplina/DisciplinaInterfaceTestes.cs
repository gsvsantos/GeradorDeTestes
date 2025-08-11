using GeradorDeTestes.Testes.InterfaceE2E.Compartilhado;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace GeradorDeTestes.Testes.InterfaceE2E.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de Interface de Disciplina")]
public sealed class DisciplinaInterfaceTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arrange
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        IWebElement elemento = driver.FindElement(By.CssSelector("a[data-se=btnCadastrar]"));

        elemento.Click();

        // Act
        driver.FindElement(By.Id("Nome")).SendKeys("Matemática");

        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        ReadOnlyCollection<IWebElement> elementosCard = driver.FindElements(By.CssSelector(".card"));

        Assert.AreEqual(1, elementosCard.Count);
    }

    [TestMethod]
    public void Deve_Editar_Disciplina_Corretamente()
    {
        // Arrange
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        IWebElement elemento = driver.FindElement(By.CssSelector("a[data-se=btnCadastrar]"));

        elemento.Click();
        driver.FindElement(By.Id("Nome")).SendKeys("Matemática");

        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        driver.FindElement(By.CssSelector(".card"))
            .FindElement(By.CssSelector("a[title='Editar Disciplina']")).Click();

        // Act
        driver.FindElement(By.Id("Nome")).SendKeys(" Editada");

        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        bool conseguiuEditar = driver.PageSource.Contains("Matemática Editada");

        Assert.IsTrue(conseguiuEditar);
    }
}
