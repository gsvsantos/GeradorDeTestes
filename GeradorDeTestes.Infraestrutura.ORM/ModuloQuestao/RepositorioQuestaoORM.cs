using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
public class RepositorioQuestaoORM : RepositorioBaseORM<Questao>, IRepositorioQuestao
{
    public RepositorioQuestaoORM(GeradorDeTestesDbContext contexto) : base(contexto) { }

    public override List<Questao> SelecionarRegistros()
    {
        return registros.Include(q => q.Materia)
            .OrderBy(q => q.Materia.Serie)
            .ThenBy(q => q.Enunciado)
            .ToList();
    }
}
