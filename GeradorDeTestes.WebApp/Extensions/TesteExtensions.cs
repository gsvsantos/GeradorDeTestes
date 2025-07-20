using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Models;

namespace GeradorDeTestes.WebApp.Extensions;

public static class TesteExtensions
{
    public static Teste ParaEntidade(this FormularioTesteViewModel formularioVM, Disciplina disciplina)
    {
        return new(
            formularioVM.Titulo,
            disciplina,
            formularioVM.Serie,
            formularioVM.EhProvao,
            formularioVM.QuantidadeQuestoes);
    }

    public static DetalhesTesteViewModel ParaDetalhesVM(this Teste teste)
    {
        return new(
            teste.Id,
            teste.Titulo,
            teste.Disciplina.Nome,
            teste.Materias,
            teste.EhProvao,
            teste.QuantidadeQuestoes);
    }
}
