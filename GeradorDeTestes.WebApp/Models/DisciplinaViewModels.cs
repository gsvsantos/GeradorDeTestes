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
