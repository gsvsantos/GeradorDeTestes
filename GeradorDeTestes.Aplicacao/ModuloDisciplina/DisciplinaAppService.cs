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

    public DisciplinaAppService(IUnitOfWork unitOfWork, IRepositorioDisciplina repositorioDisciplina,
        IRepositorioMateria repositorioMateria, IRepositorioTeste repositorioTeste,
        ILogger<DisciplinaAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioDisciplina = repositorioDisciplina;
        this.repositorioMateria = repositorioMateria;
        this.repositorioTeste = repositorioTeste;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Disciplina novaDisciplina)
    {
        List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

        if (disciplinas.Any(d => d.Nome.Equals(novaDisciplina.Nome)))
            return Result.Fail("Já existe uma disciplina com este nome.");

        try
        {
            repositorioDisciplina.CadastrarRegistro(novaDisciplina);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro de {@ViewModel}.",
                novaDisciplina
            );
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Disciplina disciplinaEditada)
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

    public Result ExcluirRegistro(Guid id)
    {
        try
        {
            Disciplina disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(id)!;

            List<Materia> materias = repositorioMateria.SelecionarRegistros();

            List<Teste> testes = repositorioTeste.SelecionarRegistros();

            if (materias.Any(m => m.Disciplina.Id == id) ||
                testes.Any(t => t.Disciplina.Id == id))
            {
                return Result.Fail("Não é possível excluir: há matérias ou testes vinculados.");
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
                "Ocorreu um erro durante a exclusão da disciplina {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar excluir a disciplina.");
        }
    }

    public Result<Disciplina> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Disciplina disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(id)!;

            if (disciplinaSelecionada is null)
                return Result.Fail("Não foi possível obter o registro da disciplina selecionada.");

            return Result.Ok(disciplinaSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da disciplina {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter a disciplina.");
        }
    }

    public Result<List<Disciplina>> SelecionarRegistros()
    {
        try
        {
            List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

            return Result.Ok(disciplinas);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção das disciplinas registradas."
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter as disciplinas.");
        }
    }
}