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
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return this;
    }

    public DisciplinaFormPageObject ClickCadastrar()
    {
        driver.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Click();

        return new(driver);
    }

    public DisciplinaFormPageObject ClickEditar()
    {
        driver.FindElement(By.CssSelector(".card a[title='Editar Disciplina']")).Click();

        return new(driver);
    }

    public DisciplinaFormPageObject ClickExcluir()
    {
        driver.FindElement(By.CssSelector(".card a[title='Excluir Disciplina']")).Click();

        return new(driver);
    }

    public bool ContemDisciplina(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}
