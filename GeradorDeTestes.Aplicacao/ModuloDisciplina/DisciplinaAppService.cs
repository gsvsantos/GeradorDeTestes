using FluentResults;
using GeradorDeTestes.Aplicacao.Compartilhado;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloDisciplina;

public class DisciplinaAppService
{
    private readonly IGeradorDisciplinas geradorDisciplinas;
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioDisciplina repositorioDisciplina;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly ILogger<DisciplinaAppService> logger;

    public DisciplinaAppService(IGeradorDisciplinas geradorDisciplinas, IUnitOfWork unitOfWork,
        IRepositorioDisciplina repositorioDisciplina, IRepositorioMateria repositorioMateria,
        IRepositorioTeste repositorioTeste, ILogger<DisciplinaAppService> logger)
    {
        this.geradorDisciplinas = geradorDisciplinas;
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
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma disciplina com este nome.");

            return Result.Fail(erro);
        }

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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Disciplina disciplinaEditada)
    {
        List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

        if (disciplinas.Any(d => d.Nome.Equals(disciplinaEditada.Nome) && d.Id != id))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma disciplina com este nome.");

            return Result.Fail(erro);
        }

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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
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

            if (materias.Any(m => m.Disciplina.Id.Equals(id) &&
                testes.Any(t => t.Disciplina.Id.Equals(id))))
            {
                Error erro = ResultadosErro.RegistroVinculadoErro(
                    $"Não foi possível excluir a disciplina '{disciplinaSelecionada.Nome}' pois ela está vinculada a matérias e testes.");

                return Result.Fail(erro);
            }
            else if (materias.Any(m => m.Disciplina.Id.Equals(id)))
            {
                Error erro = ResultadosErro.RegistroVinculadoErro(
                    $"Não foi possível excluir a disciplina '{disciplinaSelecionada.Nome}' pois ela está vinculada a matérias.");

                return Result.Fail(erro);
            }
            else if (testes.Any(t => t.Disciplina.Id.Equals(id)))
            {
                Error erro = ResultadosErro.RegistroVinculadoErro(
                    $"Não foi possível excluir a disciplina '{disciplinaSelecionada.Nome}' pois ela está vinculada a testes.");

                return Result.Fail(erro);
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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Disciplina> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Disciplina disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(id)!;

            if (disciplinaSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            return Result.Ok(disciplinaSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da disciplina {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
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

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public async Task<Result<List<Disciplina>>> GerarDisciplinas(int quantidade)
    {
        try
        {
            List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

            List<Disciplina> disciplinasGeradas = await geradorDisciplinas.GerarDisciplinasAsync(quantidade, disciplinas);

            if (disciplinasGeradas.Count == 0)
            {
                Error erro = ResultadosErro.NenhumRegistroGeradoErro(
                    "Nenhuma nova disciplina foi gerada. Tente novamente.");

                return Result.Fail(erro);
            }

            return Result.Ok(disciplinasGeradas);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a geração das disciplinas."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}