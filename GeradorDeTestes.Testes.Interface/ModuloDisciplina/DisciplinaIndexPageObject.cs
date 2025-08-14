using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GeradorDeTestes.Testes.Interface.ModuloDisciplina;
public class DisciplinaIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DisciplinaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public DisciplinaIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(new Uri(new Uri(enderecoBase.TrimEnd('/') + "/"), "disciplinas"));

        wait.Until(d => d.Url.Contains("/disciplinas", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return this;
    }

    public DisciplinaFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }

    public DisciplinaFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Editar Disciplina']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }

    public DisciplinaFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Excluir Disciplina']"))).Click();
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);

        return new(driver);
    }

    public bool ContemDisciplina(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}
