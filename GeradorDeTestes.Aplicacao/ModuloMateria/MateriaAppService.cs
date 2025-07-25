using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloMateria;
public class MateriaAppService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioDisciplina repositorioDisciplina;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioQuestao repositorioQuestao;
    private readonly ILogger<DisciplinaAppService> logger;

    public MateriaAppService(IUnitOfWork unitOfWork, IRepositorioDisciplina repositorioDisciplina,
        IRepositorioMateria repositorioMateria, IRepositorioQuestao repositorioQuestao,
        ILogger<DisciplinaAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioDisciplina = repositorioDisciplina;
        this.repositorioMateria = repositorioMateria;
        this.repositorioQuestao = repositorioQuestao;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Materia materia)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros();

        if (materias.Any(m => m.Nome.Equals(materia.Nome)
        && m.Disciplina.Id.Equals(materia.Disciplina.Id) && m.Serie.Equals(materia.Serie)))
        {
            return Result.Fail("Já existe uma matéria com este nome para a mesma disciplina e série.");
        }

        try
        {
            repositorioMateria.CadastrarRegistro(materia);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro de {@ViewModel}.",
                materia
            );
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Materia materiaEditada)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros();

        if (materias.Any(m => m.Nome.Equals(materiaEditada.Nome)
        && m.Disciplina.Id.Equals(materiaEditada.Disciplina) && m.Serie.Equals(materiaEditada.Serie) && m.Id != id))
        {
            return Result.Fail("Já existe outra matéria com este nome para a mesma disciplina e série.");
        }

        try
        {
            repositorioMateria.EditarRegistro(id, materiaEditada);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição de {@ViewModel}.",
                materiaEditada
            );
        }

        return Result.Ok();
    }

    public Result ExcluirRegistro(Guid id)
    {
        Materia materia = repositorioMateria.SelecionarRegistroPorId(id)!;
        List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

        try
        {
            if (questoes.Any(q => q.Materia.Id.Equals(id)))
                return Result.Fail("Não é possível excluir a matéria pois ela possui questões associadas.");

            repositorioMateria.ExcluirRegistro(id);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão da matéria {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar excluir a matéria.");
        }

    }

    public Result<Materia> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(id)!;

            if (materiaSelecionada is null)
                return Result.Fail("Não foi possível obter o registro da matéria selecionada.");

            return Result.Ok(materiaSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da matéria {Id}.",
                id
            );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter a matéria.");
        }
    }

    public Result<List<Materia>> SelecionarRegistros()
    {
        try
        {
            List<Materia> materias = repositorioMateria.SelecionarRegistros();

            return Result.Ok(materias);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção das matérias registradas."
                );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter as matérias.");
        }
    }
}
