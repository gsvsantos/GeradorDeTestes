namespace GeradorDeTestes.Dominio.Compartilhado;

public interface IUnitOfWork
{
    public void Commit();
    public void Rollback();
}
