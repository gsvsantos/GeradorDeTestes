using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloTeste;
public class RepositorioTesteORM : RepositorioBaseORM<Teste>, IRepositorioTeste
{
    public RepositorioTesteORM(GeradorDeTestesDbContext contexto) : base(contexto) { }

    public void AtualizarQuantidadePorMateria(Teste teste, Materia materia)
    {
        TesteMateriaQuantidade? objComQuantidade = teste.QuantidadesPorMateria
            .FirstOrDefault(x => x.Materia.Id == materia.Id);

        if (objComQuantidade is not null)
        {
            objComQuantidade.QuantidadeQuestoes++;
        }
        else
        {
            objComQuantidade = new TesteMateriaQuantidade
            {
                Id = Guid.NewGuid(),
                Materia = materia,
                QuantidadeQuestoes = 1
            };

            teste.QuantidadesPorMateria.Add(objComQuantidade);
            contexto.QuantidadesPorMateria.Add(objComQuantidade);
        }
        contexto.SaveChanges();
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

    public void RemoverRegistros(List<Teste> testes)
    {
        contexto.Testes.RemoveRange(testes);

        contexto.SaveChanges();
    }

    public override Teste? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.Include(t => t.Disciplina)
            .Include(t => t.Materias)
            .ThenInclude(m => m.Questoes)
            .Include(t => t.Questoes)
            .ThenInclude(q => q.Alternativas)
            .Include(t => t.QuantidadesPorMateria)
            .ThenInclude(qpm => qpm.Materia)
            .FirstOrDefault(t => t.Id.Equals(idRegistro));
    }

    public override List<Teste> SelecionarRegistros()
    {
        return registros.Include(t => t.Disciplina)
            .Include(t => t.Materias)
            .ThenInclude(m => m.Questoes)
            .Include(t => t.Questoes)
            .ThenInclude(q => q.Alternativas)
            .Include(t => t.QuantidadesPorMateria)
            .ThenInclude(qpm => qpm.Materia)
            .ToList();
    }
}
