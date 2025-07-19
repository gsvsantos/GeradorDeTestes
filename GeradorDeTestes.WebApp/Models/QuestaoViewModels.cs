using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioQuestaoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Digite o Enunciado.")]
    [DisplayName("Enunciado")]
    public string Enunciado { get; set; }
    public Guid? MateriaId { get; set; }

    [Required(ErrorMessage = "Escolha uma matéria.")]
    public List<SelectListItem> Materias { get; set; } = new List<SelectListItem>();
}

public class CadastrarQuestaoViewModel : FormularioQuestaoViewModel
{
    public CadastrarQuestaoViewModel() { }
    public CadastrarQuestaoViewModel(List<Materia> materias) : this()
    {
        foreach (Materia m in materias)
        {
            Materias.Add(new()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            });
        }
    }
}

public class VisualizarQuestoesViewModel
{
    public List<DetalhesQuestaoViewModel> Registros { get; set; } = new List<DetalhesQuestaoViewModel>();

    public VisualizarQuestoesViewModel(List<Questao> questoes)
    {
        foreach (Questao q in questoes)
        {
            Registros.Add(q.ParaDetalhesVM());
        }
    }
}

public class EditarQuestaoViewModel : FormularioQuestaoViewModel
{
    public EditarQuestaoViewModel() { }
    public EditarQuestaoViewModel(Questao questao, List<Materia> materias) : this()
    {
        Id = questao.Id;
        Enunciado = questao.Enunciado;
        MateriaId = questao.Materia != null ? questao.Materia.Id : Guid.Empty;
        foreach (Materia m in materias)
        {
            Materias.Add(new()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            });
        }
    }
}

public class ExcluirQuestaoViewModel : FormularioQuestaoViewModel
{
    public ExcluirQuestaoViewModel() { }
    public ExcluirQuestaoViewModel(Guid id, string enunciado) : this()
    {
        Id = id;
        Enunciado = enunciado;
    }
}

public class GerenciarAlternativasViewModel
{
    public DetalhesQuestaoViewModel Questao { get; set; }
    public List<AlternativaQuestaoViewModel> Alternativas { get; set; } = new List<AlternativaQuestaoViewModel>();
    public string TextoAlternativa { get; set; }

    public GerenciarAlternativasViewModel() { }
    public GerenciarAlternativasViewModel(Questao questao, List<Alternativa> alternativas) : this()
    {
        Questao = questao.ParaDetalhesVM();
        foreach (Alternativa a in alternativas)
        {
            Alternativas.Add(new(
                a.Id,
                a.Texto,
                a.EstaCorreta));
        }
    }
}

public class AdicionarAlternativaViewModel
{
    public string TextoAlternativa { get; set; }
}

public class DetalhesQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Enunciado { get; set; }
    public string NomeMateria { get; set; }
    public string NomeDisciplina { get; set; }
    public List<AlternativaQuestaoViewModel> Alternativas { get; set; } = new List<AlternativaQuestaoViewModel>();
    public List<TesteQuestaoViewModel> Testes { get; set; } = new List<TesteQuestaoViewModel>();

    public DetalhesQuestaoViewModel(Guid id, string enunciado, string nomeMateria, string nomeDisciplina, List<Alternativa> alternativas, List<Teste> testes)
    {
        Id = id;
        Enunciado = enunciado;
        NomeMateria = nomeMateria;
        NomeDisciplina = nomeDisciplina;
        foreach (Alternativa a in alternativas)
        {
            Alternativas.Add(new(
                a.Id,
                a.Texto,
                a.EstaCorreta));
        }
        foreach (Teste t in testes)
        {
            Testes.Add(new(
                t.Id,
                t.Titulo,
                t.Serie.GetDisplayName()));
        }
    }
}

public class AlternativaQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Texto { get; set; }
    public bool EstaCorreta { get; set; }

    public AlternativaQuestaoViewModel(Guid id, string texto, bool estaCorreta)
    {
        Texto = texto;
        Id = id;
        EstaCorreta = estaCorreta;
    }
}

public class TesteQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Serie { get; set; }

    public TesteQuestaoViewModel(Guid id, string titulo, string serie)
    {
        Id = id;
        Titulo = titulo;
        Serie = serie;
    }
}
