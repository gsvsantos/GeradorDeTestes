using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.WebApp.Models;

namespace GeradorDeTestes.WebApp.Extensions;

public static class QuestaoExtensions
{
    public static Questao ParaEntidade(this FormularioQuestaoViewModel formularioVM, Materia materia)
    {
        return new(
            formularioVM.Enunciado,
            materia);
    }

    public static DetalhesQuestaoViewModel ParaDetalhesVM(this Questao questao)
    {
        string serieFormatada = (int)questao.Materia.Serie >= 10
            ? questao.Materia.Serie.GetDisplayName()[..8] : questao.Materia.Serie.GetDisplayName()[..7];

        return new(
            questao.Id,
            questao.Enunciado,
            $"{questao.Materia.Nome} - {serieFormatada}",
            questao.Materia.Disciplina.Nome,
            questao.Alternativas,
            questao.Testes);
    }
}
