using GeradorDeTestes.Testes.Interface.ModuloDisciplina;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GeradorDeTestes.Testes.Interface.Compartilhado;
public class DisciplinaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DisciplinaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public DisciplinaFormPageObject PreencherNome(string nome)
    {
        IWebElement nomeInput = driver.FindElement(By.Id("Nome"));
        nomeInput.Clear();
        nomeInput.SendKeys(nome);
        return this;
    }

    public DisciplinaIndexPageObject ClickSubmit()
    {
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        wait.Until(d => d.FindElements(By.CssSelector(".card")).Count == 1);

        return new(driver);
    }

    public DisciplinaIndexPageObject ClickSubmitExclusao()
    {
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        wait.Until(d => d.FindElements(By.CssSelector(".card")).Count == 0);

        return new(driver);
    }
}
