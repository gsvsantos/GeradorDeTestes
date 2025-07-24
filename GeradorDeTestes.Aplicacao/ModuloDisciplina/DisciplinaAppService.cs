using FluentResults;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloDisciplina;

public class DisciplinaAppService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioDisciplina repositorioDisciplina;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly ILogger<DisciplinaAppService> logger;

    public DisciplinaAppService(IUnitOfWork unitOfWork, IRepositorioDisciplina repositorioDisciplina, IRepositorioMateria repositorioMateria, IRepositorioTeste repositorioTeste, ILogger<DisciplinaAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioDisciplina = repositorioDisciplina;
        this.repositorioMateria = repositorioMateria;
        this.repositorioTeste = repositorioTeste;
        this.logger = logger;
    }

    public Result Cadastrar(Disciplina disciplina)
    {
        if (repositorioDisciplina.SelecionarRegistros().Any(d => d.Nome.Equals(disciplina.Nome)))
            return Result.Fail("Já existe uma disciplina com este nome.");

        try
        {
            repositorioDisciplina.CadastrarRegistro(disciplina);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@ViewModel}.",
                disciplina
            );
        }

        return Result.Ok();
    }

    public Result Editar(Guid id, Disciplina disciplinaEditada)
    {
        List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

        if (disciplinas.Any(d => d.Nome.Equals(disciplinaEditada.Nome) && d.Id != id))
            return Result.Fail("Já existe uma disciplina com este nome.");

        try
        {
            repositorioDisciplina.EditarRegistro(id, disciplinaEditada);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição de {@ViewModel}.",
                disciplinaEditada
            );
        }

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Disciplina disciplina = repositorioDisciplina.SelecionarRegistroPorId(id)!;
        List<Materia> materias = repositorioMateria.SelecionarRegistros();
        List<Teste> testes = repositorioTeste.SelecionarRegistros();

        try
        {
            if (materias.Any(m => m.Disciplina.Id == id) ||
                testes.Any(t => t.Disciplina.Id == id))
            {
                return Result.Fail("Não é possível excluir: há matérias ou testes vinculados");
            }

            repositorioDisciplina.ExcluirRegistro(id);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão do registro {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar excluir o registro.");
        }
    }

    public Result<Disciplina> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Disciplina registroSelecionado = repositorioDisciplina.SelecionarRegistroPorId(id)!;

            if (registroSelecionado is null)
                return Result.Fail("Não foi possível obter o registro.");

            return Result.Ok(registroSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do registro {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter o registro.");
        }
    }

    public Result<List<Disciplina>> SelecionarRegistros()
    {
        try
        {
            List<Disciplina> registros = repositorioDisciplina.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de registros."
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter os registros.");
        }
    }
}