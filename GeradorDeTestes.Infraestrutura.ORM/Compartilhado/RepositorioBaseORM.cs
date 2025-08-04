using GeradorDeTestes.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
public class RepositorioBaseORM<T> where T : EntidadeBase<T>
{
    public readonly DbSet<T> registros;
    protected readonly GeradorDeTestesDbContext contexto;

    public RepositorioBaseORM(GeradorDeTestesDbContext contexto)
    {
        this.contexto = contexto;
        registros = contexto.Set<T>();
    }

    public virtual void CadastrarRegistro(T novoRegistro)
    {
        novoRegistro.Id = Guid.NewGuid();
        registros.Add(novoRegistro);
    }

    public void CadastrarMultiplosRegistros(IList<T> entidades)
    {
        foreach (T ent in entidades)
        {
            ent.Id = Guid.NewGuid();
        }

        registros.AddRange(entidades);
    }

    public virtual bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        T? registroSelecionado = SelecionarRegistroPorId(idRegistro);

        if (registroSelecionado is null)
            return false;

        registroSelecionado.AtualizarRegistro(registroEditado);

        return true;
    }

    public virtual bool ExcluirRegistro(Guid idRegistro)
    {
        T? registroSelecionado = SelecionarRegistroPorId(idRegistro);

        if (registroSelecionado is null)
            return false;

        registros.Remove(registroSelecionado);

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
