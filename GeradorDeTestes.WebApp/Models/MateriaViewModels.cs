using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioMateriaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Escreva um Nome.")]
    [DisplayName("Nome")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Escolha uma Disciplina.")]
    [DisplayName("Disciplina")]
    public Guid DisciplinaId { get; set; }
    public List<SelectListItem> Disciplinas { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Escolha uma Série/Ano.")]
    [DisplayName("Série")]
    public EnumSerie Serie { get; set; }
}

public class VisualizarMateriaViewModel
{
    public List<DetalhesMateriaViewModel> Registros { get; set; } = new List<DetalhesMateriaViewModel>();

    public VisualizarMateriaViewModel(List<Materia> materias)
    {
        foreach (Materia materia in materias)
        {
            Registros.Add(new DetalhesMateriaViewModel(
                materia.Id,
                materia.Nome,
                materia.Disciplina,
                materia.Serie,
                materia.Questoes));
        }
    }
}

public class CadastrarMateriaViewModel : FormularioMateriaViewModel
{
    public CadastrarMateriaViewModel() { }

    public CadastrarMateriaViewModel(List<Disciplina> disciplinas)
    {
        foreach(Disciplina disciplina in disciplinas)
        {
            SelectListItem d = new SelectListItem()
            {
                Text = disciplina.Nome,
                Value = disciplina.Id.ToString()
            };

            Disciplinas.Add(d);
        }

    }
}

public class DetalhesMateriaViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public Disciplina Disciplina { get; set; }
    public EnumSerie Serie { get; set; }
    public List<QuestaoMateriaViewModel> Questoes { get; set; } = new List<QuestaoMateriaViewModel>();

    public DetalhesMateriaViewModel(Guid id, string nome, Disciplina disciplina, EnumSerie serie, List<Questao> questoes)
    {
        Id = id;
        Nome = nome;
        Disciplina = disciplina;
        Serie = serie;

        foreach (Questao questao in questoes)
        {
            Questoes.Add(new QuestaoMateriaViewModel
                (
                    questao.Id,
                    questao.Enunciado
                ));
        }
    }
}

public class QuestaoMateriaViewModel
{
    public Guid Id { get; set;}
    public string Enunciado { get; set; }

    public QuestaoMateriaViewModel(Guid id, string enunciado)
    {
        Id = id;
        Enunciado = enunciado;
    }
}