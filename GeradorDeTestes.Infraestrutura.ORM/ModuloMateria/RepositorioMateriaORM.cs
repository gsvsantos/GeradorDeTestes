using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;

public class RepositorioMateriaORM : RepositorioBaseORM<Materia>, IRepositorio<Materia>
{
    public RepositorioMateriaORM(GeradorDeTestesDbContext contexto) : base(contexto)
    {
    }
}
