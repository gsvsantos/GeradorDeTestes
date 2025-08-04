using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina;

public class RepositorioDisciplinaORM : RepositorioBaseORM<Disciplina>, IRepositorioDisciplina
{
    public RepositorioDisciplinaORM(GeradorDeTestesDbContext contexto) : base(contexto)
    {
    }
    public override List<Disciplina> SelecionarRegistros()
    {
        return contexto.Disciplinas
            .Include(d => d.Materias)
            .Include(d => d.Testes)
            .OrderBy(t => t.Nome)
            .ToList();
    }

    public override Disciplina? SelecionarRegistroPorId(Guid idRegistro)
    {
        return contexto.Disciplinas
            .Include(d => d.Materias)
            .Include(d => d.Testes)
            .ThenInclude(t => t.Materias)
            .FirstOrDefault(d => d.Id == idRegistro);
    }
}
