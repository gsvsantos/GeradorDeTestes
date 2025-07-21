using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.WebApp.Models;

namespace GeradorDeTestes.WebApp.Extensions;

public static class MateriaExtensions
{
    public static Materia ParaEntidade(this FormularioMateriaViewModel formularioVM, Disciplina disciplina)
    {
        return new Materia(formularioVM.Nome, disciplina, formularioVM.Serie);
    }

    public static DetalhesMateriaViewModel ParaDetalhesVM(this Materia materia)
    {
        return new DetalhesMateriaViewModel(
            materia.Id,
            materia.Nome,
            materia.Disciplina.Nome,
            materia.Serie,
            materia.Questoes);
    }
}
