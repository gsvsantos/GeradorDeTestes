namespace GeradorDeTestes.Dominio.Compartilhado;
public interface IRepositorio<Tipo> where Tipo : EntidadeBase<Tipo>
{
    public void CadastrarRegistro(Tipo novoRegistro);

    public bool EditarRegistro(Guid idRegistro, Tipo registroEditado);

    public bool ExcluirRegistro(Guid idRegistro);

    public List<Tipo> SelecionarRegistros();

    public Tipo? SelecionarRegistroPorId(Guid idRegistro);
}
