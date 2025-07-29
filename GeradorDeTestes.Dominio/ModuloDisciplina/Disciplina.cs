using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;

namespace GeradorDeTestes.Dominio.ModuloDisciplina;
public class Disciplina : EntidadeBase<Disciplina>
{
    public string Nome { get; set; }
    public List<Materia> Materias { get; set; } = new List<Materia>();
    public List<Teste> Testes { get; set; } = new List<Teste>();

    public Disciplina() { }
    public Disciplina(string nome) : this()
    {
        Nome = nome;
    }

    public void AderirMateria(Materia materia)
    {
        Materias.Add(materia);
    }

    public void RemoverMateria(Materia materia)
    {
        Materias.Remove(materia);
    }

    public void AderirTeste(Teste teste)
    {
        Testes.Add(teste);
    }

    public void RemoverTeste(Teste teste)
    {
        Testes.Remove(teste);
    }

    public override void AtualizarRegistro(Disciplina registroEditado)
    {
        Nome = registroEditado.Nome;
    }
}
