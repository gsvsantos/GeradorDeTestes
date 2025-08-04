using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloMateria;

public class RepositorioMateriaORM : RepositorioBaseORM<Materia>, IRepositorioMateria
{
    public RepositorioMateriaORM(GeradorDeTestesDbContext contexto) : base(contexto) { }

    public override List<Materia> SelecionarRegistros()
    {
        return registros.Include(mate => mate.Questoes)
            .Include(mate => mate.Disciplina)
            .OrderBy(mate => mate.Nome)
            .ToList();
    }
    public override Materia? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Where(mate => mate.Id.Equals(idRegistro))
            .Include(mate => mate.Questoes)
            .Include(mate => mate.Disciplina)
            .FirstOrDefault();
    }
}
