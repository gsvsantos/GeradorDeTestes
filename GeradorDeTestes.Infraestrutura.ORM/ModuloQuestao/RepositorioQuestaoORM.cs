using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloQuestao;
public class RepositorioQuestaoORM : RepositorioBaseORM<Questao>, IRepositorioQuestao
{
    public RepositorioQuestaoORM(GeradorDeTestesDbContext contexto) : base(contexto) { }

    public Alternativa? SelecionarAlternativa(Questao questao, Guid idAlternativa)
    {
        return questao.Alternativas.FirstOrDefault(a => a.Id.Equals(idAlternativa));
    }

    public override Questao? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Include(q => q.Materia)
            .ThenInclude(m => m.Disciplina)
            .Include(q => q.Alternativas)
            .FirstOrDefault(c => c.Id.Equals(idRegistro));
    }

    public override List<Questao> SelecionarRegistros()
    {
        return registros.Include(q => q.Materia)
            .ThenInclude(m => m.Disciplina)
            .Include(q => q.Alternativas)
            .OrderBy(q => q.Materia.Serie)
            .ThenBy(q => q.Enunciado)
            .ToList();
    }
}
