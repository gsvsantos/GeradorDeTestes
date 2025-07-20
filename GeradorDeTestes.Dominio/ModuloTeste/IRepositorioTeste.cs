using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;

namespace GeradorDeTestes.Dominio.ModuloTeste;
public interface IRepositorioTeste : IRepositorio<Teste>
{
    public void AtualizarQuantidadePorMateria(Teste teste, Materia materia);
}
