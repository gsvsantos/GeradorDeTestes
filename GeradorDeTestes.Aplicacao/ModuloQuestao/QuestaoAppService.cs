using FluentResults;
using GeradorDeTestes.Aplicacao.Compartilhado;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloQuestao;
public class QuestaoAppService
{
    private readonly IGeradorQuestoes geradorQuestoes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioQuestao repositorioQuestao;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly ILogger<DisciplinaAppService> logger;

    public QuestaoAppService(IGeradorQuestoes geradorQuestoes, IUnitOfWork unitOfWork,
        IRepositorioQuestao repositorioQuestao, IRepositorioTeste repositorioTeste,
        ILogger<DisciplinaAppService> logger)
    {
        this.geradorQuestoes = geradorQuestoes;
        this.unitOfWork = unitOfWork;
        this.repositorioQuestao = repositorioQuestao;
        this.repositorioTeste = repositorioTeste;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Questao novaQuestao)
    {
        List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

        if (questoes.Any(q => q.Enunciado.Equals(novaQuestao.Enunciado)
        && q.Materia.Id.Equals(novaQuestao.Materia.Id)))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma questão com este enunciado para a mesma matéria.");

            return Result.Fail(erro);
        }

        try
        {
            repositorioQuestao.CadastrarRegistro(novaQuestao);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro de {@ViewModel}.",
                novaQuestao
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Questao questaoEditada)
    {
        List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

        if (questoes.Any(q => q.Enunciado.Equals(questaoEditada.Enunciado)
        && q.Materia.Id.Equals(questaoEditada.Materia.Id) && q.Id != id))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe uma questão com este enunciado para a mesma matéria.");

            return Result.Fail(erro);
        }

        try
        {
            repositorioQuestao.EditarRegistro(id, questaoEditada);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição de {@ViewModel}.",
                questaoEditada
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result ExcluirRegistro(Guid id)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

            List<Teste> testes = repositorioTeste.SelecionarRegistros();

            if (testes.Any(t => t.Questoes.Any(q => q.Id.Equals(id))))
            {
                Error erro = ResultadosErro.RegistroVinculadoErro(
                    "Não é possível excluir a questão pois ela está vinculada a testes.");

                return Result.Fail(erro);
            }

            repositorioQuestao.ExcluirRegistro(id);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão da questão {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Questao> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

            if (questaoSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            return Result.Ok(questaoSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da questão {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Questao>> SelecionarRegistros()
    {
        try
        {
            List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

            return Result.Ok(questoes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção das questões registradas."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Questao>> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo)
    {
        try
        {
            List<Questao> questoes = repositorioQuestao.SelecionarNaoFinalizadosAntigos(tempoMaximo);

            return Result.Ok(questoes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção das questões antigas não finalizadas."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Questao>> SelecionarNaoFinalizados()
    {
        try
        {
            List<Questao> questoes = repositorioQuestao.SelecionarNaoFinalizados();

            return Result.Ok(questoes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção das questões não finalizadas."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result RemoverRegistros(List<Questao> questoes)
    {
        try
        {
            repositorioQuestao.RemoverRegistros(questoes);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a remoção das questões."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result AdicionarAlternativa(Guid idQuestao, string textoAlternativa)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (questaoSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(idQuestao);

                return Result.Fail(erro);
            }

            if (string.IsNullOrWhiteSpace(textoAlternativa))
            {
                Error erro = ResultadosErro.TextoAlternativaObrigatorioErro(
                    "O texto da alternativa é obrigatório.");

                return Result.Fail(erro);
            }
            else if (questaoSelecionada.Alternativas.Any(a => a.Texto.Equals(textoAlternativa)))
            {
                Error erro = ResultadosErro.AlternativaDuplicadaErro(
                    "Essa alternativa já foi inserida.");

                return Result.Fail(erro);
            }
            else if (questaoSelecionada.Alternativas.Count >= 4)
            {
                Error erro = ResultadosErro.AlternativasErro(
                    "A questão já possui o máximo de 4 alternativas.");

                return Result.Fail(erro);
            }

            Alternativa novaAlternativa = new(textoAlternativa, questaoSelecionada);

            questaoSelecionada.AderirAlternativa(novaAlternativa);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao adicionar alternativa à questão {Id}",
                idQuestao
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result RemoverAlternativa(Guid idQuestao, Guid idAlternativa)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (questaoSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(idQuestao);

                return Result.Fail(erro);
            }

            Alternativa? alternativa = repositorioQuestao.SelecionarAlternativa(questaoSelecionada, idAlternativa)!;

            if (alternativa is null)
            {
                Error erro = ResultadosErro.AlternativaNaoEncontradaErro(idAlternativa);

                return Result.Fail(erro);
            }

            questaoSelecionada.RemoverAlternativa(alternativa);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao remover alternativa da questão {Id}",
                idQuestao
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result MarcarAlternativaCorreta(Guid idQuestao, Guid idAlternativa)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (questaoSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(idQuestao);

                return Result.Fail(erro);
            }

            Alternativa? alternativa = questaoSelecionada.Alternativas.FirstOrDefault(a => a.Id.Equals(idAlternativa));

            if (alternativa is null)
            {
                Error erro = ResultadosErro.AlternativaNaoEncontradaErro(idAlternativa);

                return Result.Fail(erro);
            }

            foreach (Alternativa a in questaoSelecionada.Alternativas)
                a.EstaCorreta = a.Id == idAlternativa;

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao marcar alternativa correta da questão {Id}",
                idQuestao
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result FinalizarQuestao(Guid id)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

            if (questaoSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }
            else if (questaoSelecionada.Alternativas.Count < 2 || questaoSelecionada.Alternativas.Count > 4)
            {
                Error erro = ResultadosErro.AlternativasErro(
                    "A questão deve ter entre 2 e 4 alternativas.");

                return Result.Fail(erro);
            }
            else if (questaoSelecionada.Alternativas.Count(a => a.EstaCorreta) != 1)
            {
                Error erro = ResultadosErro.AlternativasErro(
                    "A questão deve ter exatamente uma alternativa correta.");

                return Result.Fail(erro);
            }

            questaoSelecionada.Finalizado = true;

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao finalizar a questão {Id}",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public async Task<Result<List<Questao>>> GerarQuestoesDaMateria(Materia materiaSelecionada, int quantidadeQuestoes)
    {
        try
        {
            List<Questao> questoes = await geradorQuestoes.GerarQuestoesAsync(materiaSelecionada, quantidadeQuestoes);

            return Result.Ok(questoes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a geração de questões da matéria {@Registro}.",
                materiaSelecionada
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result CadastrarQuestoesGeradas(List<Questao> questoesGeradas)
    {
        try
        {
            foreach (Questao questao in questoesGeradas)
            {
                questao.Finalizado = true;

                CadastrarRegistro(questao);
            }

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro das questões geradas."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
