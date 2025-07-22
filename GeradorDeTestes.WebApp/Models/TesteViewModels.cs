using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioTesteViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "Selecione uma disciplina.")]
    public Guid DisciplinaId { get; set; }
    public List<SelectListItem> Disciplinas { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Selecione a série.")]
    public EnumSerie Serie { get; set; }
    public bool EhProvao { get; set; }

    [Range(1, 100, ErrorMessage = "A quantidade de questões deve ser entre 1 e 100.")]
    public int QuantidadeQuestoes { get; set; }
}

public class CadastrarTesteViewModel : FormularioTesteViewModel
{
    public CadastrarTesteViewModel() { }
    public CadastrarTesteViewModel(List<Disciplina> disciplinas) : this()
    {
        foreach (Disciplina d in disciplinas)
        {
            Disciplinas.Add(new()
            {
                Text = d.Nome,
                Value = d.Id.ToString()
            });
        }
    }
}

public class VisualizarTestesViewModel
{
    public List<DetalhesTestesViewModel> Registros { get; set; } = new List<DetalhesTestesViewModel>();

    public VisualizarTestesViewModel(List<Teste> testes)
    {
        foreach (Teste t in testes)
        {
            Registros.Add(t.ParaDetalhesVM());
        }
    }
}

public class ExcluirTesteViewModel : FormularioTesteViewModel
{
    public ExcluirTesteViewModel() { }
    public ExcluirTesteViewModel(Guid id, string titulo) : this()
    {
        Id = id;
        Titulo = titulo;
    }
}

public class FormGerarViewModel : FormularioTesteViewModel
{
    public string NomeDisciplina { get; set; }
    public List<SelectListItem> MateriasSelecionadas { get; set; } = new List<SelectListItem>();
    public List<MateriaQuantidadeViewModel> QuantidadesPorMateria { get; set; } = new();
    public List<SelectListItem> Questoes { get; set; } = new List<SelectListItem>();
    public Guid MateriaId { get; set; }
    public List<SelectListItem> Materias { get; set; } = new List<SelectListItem>();
}

public class FormGerarPostViewModel : FormGerarViewModel
{
    [MinLength(1, ErrorMessage = "Selecione ao menos uma questão.")]
    public List<Guid> QuestoesSelecionadasIds { get; set; } = new List<Guid>();
}

public class DuplicarViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
    public string Titulo { get; set; }
}

public class MateriaQuantidadeViewModel
{
    public Guid MateriaId { get; set; }
    public int QuantidadeQuestoes { get; set; }
}

public class DefinirQuantidadeQuestoesViewModel : FormGerarViewModel
{
    public int QuantidadeQuestoesMateria { get; set; }

    public DefinirQuantidadeQuestoesViewModel() { }
    public DefinirQuantidadeQuestoesViewModel(int quantidadeQuestoesMateria) : this()
    {
        QuantidadeQuestoesMateria = quantidadeQuestoesMateria;
    }
}
public class DefinirQuantidadeQuestoesPostViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Selecione a matéria.")]
    public Guid MateriaId { get; set; }

    [Range(0, 100, ErrorMessage = "A quantidade deve ser entre 0 e 100.")]
    public int QuantidadeQuestoesMateria { get; set; }
}

public class DetalhesTestesViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string NomeDisciplina { get; set; }
    public List<SelectListItem> Materias { get; set; } = new List<SelectListItem>();
    public bool EhProvao { get; set; }
    public int QuantidadeQuestoes { get; set; }

    public DetalhesTestesViewModel() { }
    public DetalhesTestesViewModel(Guid id, string titulo, string nomeDisciplina, List<Materia> materias, bool ehProvao, int quantidadeQuestoes) : this()
    {
        Id = id;
        Titulo = titulo;
        NomeDisciplina = nomeDisciplina;
        foreach (Materia m in materias)
        {
            Materias.Add(new()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            });
        }
        EhProvao = ehProvao;
        QuantidadeQuestoes = quantidadeQuestoes;
    }
}

public class DetalhesTesteViewModel : DetalhesTestesViewModel
{
    public EnumSerie Serie { get; set; }
    public List<MateriaComQuestoesViewModel> MateriasComQuestoes { get; set; } = new List<MateriaComQuestoesViewModel>();
}

public class MateriaComQuestoesViewModel
{
    public string NomeMateria { get; set; }
    public List<string> Questoes { get; set; }
}

public class DetalhesProvaoViewModel : DetalhesTestesViewModel
{
    public EnumSerie Serie { get; set; }
    public List<MateriaComQuestoesViewModel> MateriasComQuestoes { get; set; } = new List<MateriaComQuestoesViewModel>();
}