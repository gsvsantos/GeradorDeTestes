using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GeradorDeTestes.Testes.Interface.ModuloMateria;
public class MateriaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public MateriaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public MateriaFormPageObject PreencherNome(string nome)
    {
        wait.Until(d =>
            d.FindElement(By.Id("Nome")).Displayed &&
            d.FindElement(By.Id("Nome")).Enabled
        );

        IWebElement nomeInput = driver.FindElement(By.Id("Nome"));
        nomeInput.Clear();
        nomeInput.SendKeys(nome);

        return this;
    }

    public MateriaFormPageObject SelecionarDisciplina(string disciplina)
    {
        wait.Until(d =>
            d.FindElement(By.Id("DisciplinaId")).Displayed &&
            d.FindElement(By.Id("DisciplinaId")).Enabled
        );

        SelectElement selectDisciplina = new(driver.FindElement(By.Id("DisciplinaId")));

        selectDisciplina.SelectByText(disciplina);

        return this;
    }

    public MateriaFormPageObject SelecionarSerie(string serie)
    {
        wait.Until(d =>
            d.FindElement(By.Id("Serie")).Displayed &&
            d.FindElement(By.Id("Serie")).Enabled
        );

        SelectElement selectSerie = new(driver.FindElement(By.Id("Serie")));

        selectSerie.SelectByText(serie);

        return this;
    }

    public MateriaIndexPageObject ClickSubmit()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();
        wait.Until(d => d.Url.Contains("/materias", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new(driver);
    }

    public MateriaIndexPageObject ClickSubmitExcluir(string nome)
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();
        wait.Until(d => d.Url.Contains("/materias", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        wait.Until(d => !d.PageSource.Contains(nome));

        return new(driver);
    }

    public bool ContemMateria(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}
