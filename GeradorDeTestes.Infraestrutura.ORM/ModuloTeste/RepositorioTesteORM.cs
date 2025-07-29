using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
public class RepositorioTesteORM : RepositorioBaseORM<Teste>, IRepositorioTeste
{
    public RepositorioTesteORM(GeradorDeTestesDbContext contexto) : base(contexto) { }

    public void AtualizarRegistro(Teste testeSelecionado)
    {
        contexto.Update(testeSelecionado);
    }

    public void AtualizarQuantidadePorMateria(Teste testeSelecionado, Materia materia)
    {
        TesteMateriaQuantidade? objComQuantidade = testeSelecionado.QuantidadesPorMateria
            .FirstOrDefault(x => x.Materia.Id == materia.Id);

        if (objComQuantidade is null)
        {
            objComQuantidade = new TesteMateriaQuantidade
            {
                Id = Guid.NewGuid(),
                Materia = materia,
                QuantidadeQuestoes = 1
            };

            testeSelecionado.QuantidadesPorMateria.Add(objComQuantidade);

            contexto.Entry(objComQuantidade).State = EntityState.Added;
        }
        else
        {
            objComQuantidade.QuantidadeQuestoes++;
        }
    }
    public void RemoverQuantidadePorMateria(TesteMateriaQuantidade quantidade)
    {
        contexto.Remove(quantidade);
    }

    public List<Teste> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo)
    {
        DateTime limite = DateTime.Now.Subtract(tempoMaximo);

        return registros.Where(t => !t.Finalizado && t.DataCriacao < limite).ToList();
    }

    public List<Teste> SelecionarNaoFinalizados()
    {
        return registros.Where(t => !t.Finalizado).ToList();
    }

    public List<Questao> SelecionarQuestoesParaProvao(Guid disciplinaId, int quantidade)
    {
        return contexto.Questoes
            .Include(q => q.Materia)
            .Where(q => q.Materia.Disciplina.Id == disciplinaId && q.Finalizado)
            .OrderBy(q => Guid.NewGuid())
            .Take(quantidade)
            .ToList();
    }

    public void RemoverRegistros(List<Teste> testes)
    {
        contexto.Testes.RemoveRange(testes);
    }

    public override Teste? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Include(t => t.Disciplina)
            .Include(t => t.Materias)
            .ThenInclude(m => m.Questoes)
            .Include(t => t.Questoes)
            .ThenInclude(q => q.Alternativas)
            .Include(t => t.Questoes)
            .ThenInclude(q => q.Materia)
            .Include(t => t.QuantidadesPorMateria)
            .ThenInclude(qpm => qpm.Materia)
            .Include(t => t.Materias)
            .ThenInclude(m => m.Disciplina)
            .FirstOrDefault(t => t.Id.Equals(idRegistro));
    }

    public override List<Teste> SelecionarRegistros()
    {
        return registros.Where(t => t.Finalizado)
            .Include(t => t.Disciplina)
            .Include(t => t.Materias)
            .ThenInclude(m => m.Questoes)
            .Include(t => t.Questoes)
            .ThenInclude(q => q.Alternativas)
            .Include(t => t.QuantidadesPorMateria)
            .ThenInclude(qpm => qpm.Materia)
            .ToList();
    }
}
