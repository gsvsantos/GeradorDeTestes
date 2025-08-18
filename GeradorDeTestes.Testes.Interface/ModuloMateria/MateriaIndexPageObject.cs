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
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/materias");

        wait.Until(d => d.Url.Contains("/materias", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return this;
    }

    public MateriaFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }

    public MateriaFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Editar Matéria']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }

    public MateriaFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Excluir Matéria']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }
}
