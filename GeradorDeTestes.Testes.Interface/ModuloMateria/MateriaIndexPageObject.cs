using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GeradorDeTestes.Testes.Interface.ModuloMateria;
public class MateriaIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public MateriaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public MateriaIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "materias"));

        return this;
    }

    public MateriaFormPageObject ClickCadastrar()
    {
        driver.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Click();

        return new(driver);
    }

    public MateriaFormPageObject ClickEditar()
    {
        driver.FindElement(By.CssSelector(".card a[title='Editar Disciplina']")).Click();

        return new(driver);
    }

    public MateriaFormPageObject ClickExcluir()
    {
        driver.FindElement(By.CssSelector(".card a[title='Excluir Disciplina']")).Click();

        return new(driver);
    }

    public bool ContemMateria(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}
