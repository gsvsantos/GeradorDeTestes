using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;

namespace GeradorDeTestes.Infraestrutura.ORM.ModuloDisciplina
{
    public class RepositorioDisciplinaORM : RepositorioBaseORM<Disciplina>, IRepositorioDisciplina
    {
        public RepositorioDisciplinaORM(GeradorDeTestesDbContext contexto) : base(contexto)
        {
        }
    }
}
