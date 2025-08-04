using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Testes.Integracao.ModuloMateria;

[TestClass]
[TestCategory("Testes de Integra��o de Mat�ria")]
public sealed class RepositorioMateriaORMTestes : TestFixture
{
    private Disciplina disciplinaPadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        disciplinaPadrao = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matem�tica")
            .Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplica��o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        // Act
        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsNotNull(materiaSelecionada, "N�o conseguiu selecionar a mat�ria.");
        Assert.AreEqual(novaMateria, materiaSelecionada, "A mat�ria selecionada n�o corresponde com a mat�ria cadastrada.");
        Assert.AreEqual(disciplinaPadrao, materiaSelecionada.Disciplina, "A disciplina n�o corresponde com a disciplina da mat�ria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, materiaSelecionada.Serie, "A s�rie n�o corresponde com a s�rie da mat�ria cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplica��o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        Disciplina novaDisciplina2 = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Aritm�tica")
            .Persist();

        Materia materiaEditada = new("Subtra��o", novaDisciplina2, EnumSerie.QuintoAnoFundamental);

        bool conseguiuEditar = repositorioMateriaORM.EditarRegistro(novaMateria.Id, materiaEditada);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsTrue(conseguiuEditar, "N�o conseguiu editar a mat�ria.");
        Assert.IsNotNull(materiaSelecionada, "N�o conseguiu selecionar a mat�ria.");
        Assert.AreEqual(novaMateria, materiaSelecionada, "A mat�ria selecionada n�o corresponde com a mat�ria editada.");
        Assert.AreEqual(materiaEditada.Disciplina, materiaSelecionada.Disciplina, "A disciplina n�o corresponde com a disciplina da mat�ria editada.");
        Assert.AreEqual(materiaEditada.Serie, materiaSelecionada.Serie, "A s�rie n�o corresponde com a s�rie da mat�ria editada.");
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplica��o", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = repositorioMateriaORM.ExcluirRegistro(novaMateria.Id);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsTrue(conseguiuExcluir, "N�o conseguiu excluir a mat�ria.");
        Assert.IsNull(materiaSelecionada, "A mat�ria ainda est� no banco ap�s exclus�o.");
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_As_Materias_Corretamente()
    {
        // Arrange
        List<Disciplina> novasDisciplinas = Builder<Disciplina>.CreateListOfSize(3).Persist().ToList();

        List<Materia> novasMaterias = new()
        {
            new Materia("Multiplic�o", novasDisciplinas[0], EnumSerie.QuintoAnoFundamental),
            new Materia("ABC e Verbos", novasDisciplinas[1], EnumSerie.PrimeiroAnoFundamental),
            new Materia("Verbo to Be", novasDisciplinas[2], EnumSerie.SetimoAnoFundamental)
        };

        foreach (Materia materia in novasMaterias)
        {
            repositorioMateriaORM.CadastrarRegistro(materia);
        }

        dbContext.SaveChanges();

        // Act 
        List<Materia> materiasExistentesOrganizadas = repositorioMateriaORM.SelecionarRegistros();
        List<Materia> novasMateriasOrganizadas = novasMaterias.OrderBy(m => m.Nome).ToList();

        // Assert
        Assert.AreEqual(novasMaterias.Count, materiasExistentesOrganizadas.Count);
        CollectionAssert.AreEquivalent(novasMateriasOrganizadas, materiasExistentesOrganizadas);
    }
}
