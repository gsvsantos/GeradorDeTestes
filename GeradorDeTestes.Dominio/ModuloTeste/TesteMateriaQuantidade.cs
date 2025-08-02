using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Dominio.ModuloTeste;

public class TesteMateriaQuantidade
{
    public Guid Id { get; set; }
    public Materia Materia { get; set; }
    public Teste Teste { get; set; }
    public int QuantidadeQuestoes { get; set; }
}