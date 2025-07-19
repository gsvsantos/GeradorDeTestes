using GeradorDeTestes.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class RepositorioBaseORM<T> where T : EntidadeBase<T>
{
    public readonly DbSet<T> registros;

    public RepositorioBaseORM(GeradorDeTestesDbContext contexto)
    {
        registros = contexto.Set<T>();
    }

    public virtual void CadastrarRegistro(T novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();
        registros.Add(novoRegistro);
    }

    public virtual bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        T? categoriaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (categoriaSelecionada is null)
            return false;

        categoriaSelecionada.AtualizarRegistro(registroEditado);

        return true;
    }

    public virtual bool ExcluirRegistro(Guid idRegistro)
    {
        T? categoriaSelecionada = SelecionarRegistroPorId(idRegistro);

        if (categoriaSelecionada is null)
            return false;

        registros.Remove(categoriaSelecionada);

        return true;
    }

    public virtual T? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros.FirstOrDefault(c => c.Id.Equals(idRegistro));
    }

    public virtual List<T> SelecionarRegistros()
    {
        return registros.ToList();
    }
}
