using FizzWare.NBuilder;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Testes.Integracao.ModuloMateria;

[TestClass]
[TestCategory("Testes de Integração de Matéria")]
public sealed class RepositorioMateriaORMTestes : TestFixture
{
    private Disciplina disciplinaPadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        disciplinaPadrao = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplicação", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        // Act
        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsNotNull(materiaSelecionada, "Não conseguiu selecionar a matéria.");
        Assert.AreEqual(novaMateria, materiaSelecionada, "A matéria selecionada não corresponde com a matéria cadastrada.");
        Assert.AreEqual(disciplinaPadrao, materiaSelecionada.Disciplina, "A disciplina não corresponde com a disciplina da matéria cadastrada.");
        Assert.AreEqual(EnumSerie.SetimoAnoFundamental, materiaSelecionada.Serie, "A série não corresponde com a série da matéria cadastrada.");
    }

    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplicação", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        Disciplina novaDisciplina2 = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Aritmética")
            .Persist();

        Materia materiaEditada = new("Subtração", novaDisciplina2, EnumSerie.QuintoAnoFundamental);

        bool conseguiuEditar = repositorioMateriaORM.EditarRegistro(novaMateria.Id, materiaEditada);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsTrue(conseguiuEditar, "Não conseguiu editar a matéria.");
        Assert.IsNotNull(materiaSelecionada, "Não conseguiu selecionar a matéria.");
        Assert.AreEqual(novaMateria, materiaSelecionada, "A matéria selecionada não corresponde com a matéria editada.");
        Assert.AreEqual(materiaEditada.Disciplina, materiaSelecionada.Disciplina, "A disciplina não corresponde com a disciplina da matéria editada.");
        Assert.AreEqual(materiaEditada.Serie, materiaSelecionada.Serie, "A série não corresponde com a série da matéria editada.");
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        Materia novaMateria = new("Multiplicação", disciplinaPadrao, EnumSerie.SetimoAnoFundamental);

        repositorioMateriaORM.CadastrarRegistro(novaMateria);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = repositorioMateriaORM.ExcluirRegistro(novaMateria.Id);

        dbContext.SaveChanges();

        // Assert
        Materia? materiaSelecionada = repositorioMateriaORM.SelecionarRegistroPorId(novaMateria.Id);

        Assert.IsTrue(conseguiuExcluir, "Não conseguiu excluir a matéria.");
        Assert.IsNull(materiaSelecionada, "A matéria ainda está no banco após exclusão.");
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_As_Materias_Corretamente()
    {
        // Arrange
        List<Disciplina> novasDisciplinas = Builder<Disciplina>.CreateListOfSize(3).Persist().ToList();

        List<Materia> novasMaterias = new()
        {
            new Materia("Multiplicão", novasDisciplinas[0], EnumSerie.QuintoAnoFundamental),
            new Materia("ABC e Verbos", novasDisciplinas[1], EnumSerie.PrimeiroAnoFundamental),
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

        // Assert
        Assert.AreEqual(novasMaterias.Count, materiasExistentesOrganizadas.Count);
        for (int i = 0; i < novasMateriasOrganizadas.Count; i++)
        {
            Materia novaMateria = novasMateriasOrganizadas[i];
            Materia materiaExistente = materiasExistentesOrganizadas[i];

            Assert.AreEqual(novaMateria.Nome, materiaExistente.Nome, $"Nome incorreto para a matéria '{novaMateria.Nome}'.");
            Assert.AreEqual(novaMateria.Serie, materiaExistente.Serie, $"Série incorreta para a matéria '{novaMateria.Nome}'.");

            Assert.IsNotNull(materiaExistente.Disciplina, $"Disciplina não carregada para a matéria '{novaMateria.Nome}'.");
            Assert.AreEqual(novaMateria.Disciplina, materiaExistente.Disciplina, $"Disciplina incorreta para a matéria '{novaMateria.Nome}'.");
        }
    }
}
