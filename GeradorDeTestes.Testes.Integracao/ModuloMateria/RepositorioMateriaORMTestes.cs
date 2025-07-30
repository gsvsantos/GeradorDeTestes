using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;
using GeradorDeTestes.Testes.Integracao.Compartilhado;

namespace GeradorDeTestes.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integra��o de Mat�ria")]
public class RepositorioMateriaORMTestes
{
    private GeradorDeTestesDbContext dbContext;
    private RepositorioDisciplinaORM repositorioDisciplinaORM;
    private RepositorioMateriaORM repositorioMateriaORM;

    [TestInitialize]
    public void ConfigurarTestes()
    {
        dbContext = TesteDbContextFactory.CriarDbContext();

        repositorioDisciplinaORM = new RepositorioDisciplinaORM(dbContext);
        repositorioMateriaORM = new RepositorioMateriaORM(dbContext);
    }

    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matem�tica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);

        Materia novaMateria = new("Multiplica��o", novaDisciplina, EnumSerie.SetimoAnoFundamental);

        // Act
        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Assert
        Materia materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id)!;

        Assert.IsNotNull(materiaSelecionada, "N�o conseguiu selecionar a mat�ria.");
        Assert.AreEqual(novaMateria.Id, materiaSelecionada.Id, "A mat�ria selecionada n�o condiz com a mat�ria cadastrada.");
        Assert.AreEqual(novaMateria.Nome, materiaSelecionada.Nome, "A mat�ria selecionada n�o condiz com a mat�ria cadastrada.");
        Assert.AreEqual(novaDisciplina.Id, materiaSelecionada.Disciplina.Id, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(novaDisciplina.Nome, materiaSelecionada.Disciplina.Nome, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, materiaSelecionada.Serie, "A s�rie n�o corresponde com a s�rie da mat�ria cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matem�tica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);

        Materia novaMateria = new("Multiplica��o", novaDisciplina, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        Disciplina novaDisciplina2 = new("Aritm�tica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina2);

        Materia materiaEditada = new("Subtra��o", novaDisciplina2, EnumSerie.QuintoAnoFundamental);

        bool conseguiuEditar = repositorioMateriaORM.EditarRegistro(novaMateria.Id, materiaEditada);

        dbContext.SaveChanges();

        // Assert
        Materia materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id)!;

        Assert.IsTrue(conseguiuEditar, "N�o conseguiu editar a mat�ria.");
        Assert.IsNotNull(materiaSelecionada, "N�o conseguiu selecionar a mat�ria.");
        Assert.AreEqual(novaMateria.Id, materiaSelecionada.Id, "A mat�ria selecionada n�o condiz com a mat�ria cadastrada.");
        Assert.AreEqual(materiaEditada.Nome, materiaSelecionada.Nome, "A mat�ria selecionada n�o condiz com a mat�ria cadastrada.");
        Assert.AreEqual(materiaEditada.Disciplina.Id, materiaSelecionada.Disciplina.Id, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(materiaEditada.Disciplina.Nome, materiaSelecionada.Disciplina.Nome, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(materiaEditada.Serie, materiaSelecionada.Serie, "A s�rie n�o corresponde com a s�rie da mat�ria cadastrada.");
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        Disciplina novaDisciplina = new("Matem�tica");

        repositorioDisciplinaORM.CadastrarRegistro(novaDisciplina);

        Materia novaMateria = new("Multiplica��o", novaDisciplina, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = repositorioMateriaORM.ExcluirRegistro(novaMateria.Id);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsNull(materiaSelecionada, "A mat�ria ainda est� no banco ap�s exclus�o.");
        Assert.IsTrue(conseguiuExcluir, "N�o conseguiu excluir a mat�ria.");
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_As_Materias_Corretamente()
    {
        // Arrange
        List<Disciplina> novasDisciplinas = new()
        {
            new Disciplina("Matem�tica"),
            new Disciplina("Portugu�s"),
            new Disciplina("Ingl�s")
        };

        foreach (Disciplina disciplina in novasDisciplinas)
        {
            repositorioDisciplinaORM.CadastrarRegistro(disciplina);
        }

        List<Materia> novasMaterias = new()
        {
            new Materia("Multiplica��o", novasDisciplinas[0], EnumSerie.QuintoAnoFundamental),
            new Materia("Verbos", novasDisciplinas[1], EnumSerie.PrimeiroAnoFundamental),
            new Materia("Verbo to Be", novasDisciplinas[2], EnumSerie.SetimoAnoFundamental)
        };

        foreach (Materia materia in novasMaterias)
        {
            repositorioMateriaORM.CadastrarRegistro(materia);
        }

        dbContext.SaveChanges();

        // Act 
        List<Materia> materiasExistentesOrganizadas = repositorioMateriaORM.SelecionarRegistros().OrderBy(m => m.Nome).ToList();
        List<Materia> novasMateriasOrganizadas = novasMaterias.OrderBy(m => m.Nome).ToList();

        Assert.AreEqual(novasMaterias.Count, materiasExistentesOrganizadas.Count);
        for (int i = 0; i < novasMateriasOrganizadas.Count; i++)
        {
            Materia novaMateria = novasMateriasOrganizadas[i];
            Materia materiaExistente = materiasExistentesOrganizadas[i];

            Assert.AreEqual(novaMateria.Nome, materiaExistente.Nome, $"Nome incorreto para a mat�ria '{novaMateria.Nome}'.");
            Assert.AreEqual(novaMateria.Serie, materiaExistente.Serie, $"S�rie incorreta para a mat�ria '{novaMateria.Nome}'.");

            Assert.IsNotNull(materiaExistente.Disciplina, $"Disciplina n�o carregada para a mat�ria '{novaMateria.Nome}'.");
            Assert.AreEqual(novaMateria.Disciplina.Id, materiaExistente.Disciplina.Id, $"Disciplina incorreta para a mat�ria '{novaMateria.Nome}'.");
            Assert.AreEqual(novaMateria.Disciplina.Nome, materiaExistente.Disciplina.Nome, $"Nome da disciplina incorreto para a mat�ria '{novaMateria.Nome}'.");
        }
    }
}
