using FluentResults;
using GeradorDeTestes.Aplicacao.Compartilhado;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloMateria;
public class MateriaAppService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioQuestao repositorioQuestao;
    private readonly ILogger<MateriaAppService> logger;

    public MateriaAppService(IUnitOfWork unitOfWork, IRepositorioMateria repositorioMateria,
        IRepositorioQuestao repositorioQuestao, ILogger<MateriaAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioMateria = repositorioMateria;
        this.repositorioQuestao = repositorioQuestao;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Materia novaMateria)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros();

        if (materias.Any(m => m.Nome.Equals(novaMateria.Nome)
        && m.Disciplina.Id.Equals(novaMateria.Disciplina.Id) && m.Serie.Equals(novaMateria.Serie)))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma matéria com este nome para a mesma disciplina e série.");

            return Result.Fail(erro);
        }

        try
        {
            repositorioMateria.CadastrarRegistro(novaMateria);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro de {@ViewModel}.",
                novaMateria
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Materia materiaEditada)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros();

        if (materias.Any(m => m.Nome.Equals(materiaEditada.Nome)
        && m.Disciplina.Id.Equals(materiaEditada.Disciplina.Id) && m.Serie.Equals(materiaEditada.Serie) && m.Id != id))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma matéria com este nome para a mesma disciplina e série.");

            return Result.Fail(erro);
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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result ExcluirRegistro(Guid id)
    {
        try
        {
            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(id)!;

            List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

            if (questoes.Any(q => q.Materia.Id.Equals(id)))
            {
                Error erro = ResultadosErro.RegistroVinculadoErro(
                    "Não é possível excluir a matéria pois ela possui questões associadas.");

                return Result.Fail(erro);
            }

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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

    }

    public Result<Materia> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(id)!;

            if (materiaSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            return Result.Ok(materiaSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da matéria {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
