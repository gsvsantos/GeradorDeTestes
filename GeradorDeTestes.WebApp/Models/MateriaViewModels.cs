using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioMateriaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Escreva um Nome.")]
    [DisplayName("Nome")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Escolha uma Disciplina.")]
    [DisplayName("Discplina")]
    public Disciplina Disciplina { get; set; }

    [Required(ErrorMessage = "Escolha uma Série/Ano.")]
    [DisplayName("Série")]
    public EnumSerie Serie { get; set; }
}

public class VisulizarMateriaViewModel
{
    public List<DetalhesMateriaViewModel> Registros { get; set; } 

    public VisulizarMateriaViewModel(List<Materia> materias)
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

    public CadastrarMateriaViewModel(Guid id, string nome, Disciplina disciplina, EnumSerie serie)
    {
        Id = id;
        Nome = nome;
        Disciplina = disciplina;
        Serie = serie;
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