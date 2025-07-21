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
    public List<Questao> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo)
    {
        DateTime limite = DateTime.Now.Subtract(tempoMaximo);

        return registros.Where(q => !q.Finalizado && q.DataCriacao < limite).ToList();
    }

    public List<Questao> SelecionarNaoFinalizados()
    {
        return registros.Where(t => !t.Finalizado).ToList();
    }

    public void RemoverRegistros(List<Questao> questoes)
    {
        contexto.Questoes.RemoveRange(questoes);
        contexto.SaveChanges();
    }

    public override Questao? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Include(q => q.Materia)
            .ThenInclude(m => m.Disciplina)
            .Include(q => q.Alternativas)
            .Include(q => q.Testes)
            .FirstOrDefault(c => c.Id.Equals(idRegistro));
    }

    public override List<Questao> SelecionarRegistros()
    {
        return registros.Include(q => q.Materia)
            .ThenInclude(m => m.Disciplina)
            .Include(q => q.Alternativas)
            .Include(q => q.Testes)
            .OrderBy(q => q.Materia.Serie)
            .ThenBy(q => q.Enunciado)
            .ToList();
    }
}
