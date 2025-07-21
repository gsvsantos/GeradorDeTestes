using GeradorDeTestes.Dominio.ModuloMateria;

public class FormularioDisciplinaViewModel
{
    public Guid Id { get; set; }
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
    public string NomeMateria { get; set; }
    public int QuantidadeQuestoes { get; set; }
}
