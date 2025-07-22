using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioMateriaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Escreva um Nome.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
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
            Registros.Add(materia.ParaDetalhesVM());
    }
}

public class CadastrarMateriaViewModel : FormularioMateriaViewModel
{
    public CadastrarMateriaViewModel() { }

    public CadastrarMateriaViewModel(List<Disciplina> disciplinas)
    {
        foreach (Disciplina disciplina in disciplinas)
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

public class EditarMateriaViewModel : FormularioMateriaViewModel
{
    public EditarMateriaViewModel() { }

    public EditarMateriaViewModel(Materia materia, List<Disciplina> disciplinas)
    {
        Id = materia.Id;
        Nome = materia.Nome;

        foreach (Disciplina disciplina in disciplinas)
        {
            SelectListItem d = new SelectListItem()
            {
                Text = disciplina.Nome,
                Value = disciplina.Id.ToString()
            };

            Disciplinas.Add(d);
        }
        DisciplinaId = materia.Disciplina.Id;
        Serie = materia.Serie;
    }
}

public class ExcluirMateriaViewModel : FormularioMateriaViewModel
{
    public ExcluirMateriaViewModel() { }

    public ExcluirMateriaViewModel(Guid id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}

public class DetalhesMateriaViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string NomeDisciplina { get; set; }
    public EnumSerie Serie { get; set; }
    public List<QuestaoMateriaViewModel> Questoes { get; set; } = new List<QuestaoMateriaViewModel>();

    public DetalhesMateriaViewModel(Guid id, string nome, string nomeDisciplina, EnumSerie serie, List<Questao> questoes)
    {
        Id = id;
        Nome = nome;
        NomeDisciplina = nomeDisciplina;
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
    public Guid Id { get; set; }
    public string Enunciado { get; set; }

    public QuestaoMateriaViewModel(Guid id, string enunciado)
    {
        Id = id;
        Enunciado = enunciado;
    }
}