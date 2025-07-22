using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public static DetalhesTestesViewModel ParaDetalhesVM(this Teste teste)
    {
        return new(
            teste.Id,
            teste.Titulo,
            teste.Disciplina.Nome,
            teste.Materias,
            teste.EhProvao,
            teste.QuantidadeQuestoes);
    }

    public static DetalhesTesteViewModel ParaDetalhesTesteVM(this Teste teste)
    {
        return new()
        {
            Id = teste.Id,
            Titulo = teste.Titulo,
            NomeDisciplina = teste.Disciplina.Nome,
            Serie = teste.Serie,
            QuantidadeQuestoes = teste.QuantidadeQuestoes,
            EhProvao = teste.EhProvao,
            Materias = teste.Materias.Select(m => new SelectListItem()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            }).ToList(),
            MateriasComQuestoes = teste.Materias.Select(m => new MateriaComQuestoesViewModel
            {
                NomeMateria = m.Nome,
                Questoes = teste.Questoes
                    .Where(q => q.Materia.Id == m.Id)
                    .Select(q => q.Enunciado)
                    .ToList()
            }).ToList()
        };
    }

    public static DetalhesProvaoViewModel ParaDetalhesProvaoVM(this Teste teste)
    {
        return new()
        {
            Id = teste.Id,
            Titulo = teste.Titulo,
            NomeDisciplina = teste.Disciplina.Nome,
            Serie = teste.Serie,
            QuantidadeQuestoes = teste.QuantidadeQuestoes,
            EhProvao = teste.EhProvao,
            Materias = teste.Materias.Select(m => new SelectListItem()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            }).ToList(),
            MateriasComQuestoes = teste.Materias.Select(m => new MateriaComQuestoesViewModel
            {
                NomeMateria = m.Nome,
                Questoes = teste.Questoes
                    .Where(q => q.Materia.Id == m.Id)
                    .Select(q => q.Enunciado)
                    .ToList()
            }).ToList()
        };
    }

    public static FormGerarPostViewModel ParaGerarTestePostVM(this Teste teste, List<Materia> materias, List<Materia> materiasSelecionadas)
    {
        return new()
        {
            Id = teste.Id,
            Titulo = teste.Titulo,
            DisciplinaId = teste.Disciplina.Id,
            NomeDisciplina = teste.Disciplina.Nome,
            Serie = teste.Serie,
            QuantidadeQuestoes = teste.QuantidadeQuestoes,
            Materias = materias.Select(m => new SelectListItem()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            }).ToList(),
            MateriasSelecionadas = materiasSelecionadas.Select(m => new SelectListItem()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            }).ToList(),
            QuantidadesPorMateria = teste.QuantidadesPorMateria.Select(qpm =>
            new MateriaQuantidadeViewModel
            {
                MateriaId = qpm.Materia.Id,
                QuantidadeQuestoes = qpm.QuantidadeQuestoes
            }).ToList(),
            Questoes = teste.Questoes.Select(q => new SelectListItem
            {
                Text = q.Enunciado,
                Value = q.Id.ToString()
            }).ToList(),
            QuestoesSelecionadasIds = teste.Questoes.Select(q => q.Id).ToList()
        };
    }
}
