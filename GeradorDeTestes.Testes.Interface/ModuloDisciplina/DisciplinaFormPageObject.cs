using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GeradorDeTestes.Testes.Interface.ModuloDisciplina;
public class DisciplinaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DisciplinaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

        return new(driver);
    }
}
