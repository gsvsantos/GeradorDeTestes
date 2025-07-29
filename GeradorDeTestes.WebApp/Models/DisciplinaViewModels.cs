using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using System.ComponentModel.DataAnnotations;

public class FormularioDisciplinaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; }
}

public class CadastrarDisciplinaViewModel : FormularioDisciplinaViewModel { }

public class EditarDisciplinaViewModel : FormularioDisciplinaViewModel { }

public class ExcluirDisciplinaViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
}

public class VisualizarDisciplinasViewModel
{
    public List<DetalhesDisciplinaViewModel> Registros { get; set; } = new();
}

public class DetalhesDisciplinaViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public List<MateriaResumoViewModel> Materias { get; set; } = new();
    public List<TesteResumoViewModel> Testes { get; set; } = new();
}

public class MateriaResumoViewModel
{
    public string Nome { get; set; }
    public EnumSerie Serie { get; set; }
}

public class TesteResumoViewModel
{
    public string Titulo { get; set; }
    public int QtdMaterias { get; set; }
    public int QuantidadeQuestoes { get; set; }
}

public class PrimeiraEtapaGerarDisciplinasViewModel
{
    [Required(ErrorMessage = "O campo \"Quantidade de Disciplinas\" é obrigatório.")]
    [Range(1, 100, ErrorMessage = "O campo \"Quantidade de Disciplinas\" precisa conter um valor numérico entre 1 e 10.")]
    public int QuantidadeDisciplinas { get; set; }
}

public class SegundaEtapaGerarDisciplinasViewModel
{
    public List<DisciplinaGeradaViewModel> DisciplinaGeradas { get; set; } = new List<DisciplinaGeradaViewModel>();

    public SegundaEtapaGerarDisciplinasViewModel() { }
    public SegundaEtapaGerarDisciplinasViewModel(List<Disciplina> disciplinas) : this()
    {
        DisciplinaGeradas = disciplinas
            .Select(DisciplinaGeradaViewModel.ParaViewModel)
            .ToList();
    }

    public static List<Disciplina> ObterDisciplinasGeradas(SegundaEtapaGerarDisciplinasViewModel segundaEtapaVM)
    {
        List<Disciplina> disciplinas = new List<Disciplina>();

        foreach (DisciplinaGeradaViewModel disciplinaVM in segundaEtapaVM.DisciplinaGeradas)
        {
            Disciplina disciplina = new()
            {
                Id = Guid.NewGuid(),
                Nome = disciplinaVM.Nome
            };

            disciplinas.Add(disciplina);
        }

        return disciplinas;
    }
}

public class DisciplinaGeradaViewModel
{
    public required string Nome { get; set; }

    public static DisciplinaGeradaViewModel ParaViewModel(Disciplina disciplina)
    {
        return new() { Nome = disciplina.Nome };
    }
}