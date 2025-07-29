using FluentResults;
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
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioQuestao repositorioQuestao;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly ILogger<DisciplinaAppService> logger;

    public QuestaoAppService(IGeradorQuestoes geradorQuestoes, IUnitOfWork unitOfWork,
        IRepositorioMateria repositorioMateria, IRepositorioQuestao repositorioQuestao,
        IRepositorioTeste repositorioTeste, ILogger<DisciplinaAppService> logger)
    {
        this.geradorQuestoes = geradorQuestoes;
        this.unitOfWork = unitOfWork;
        this.repositorioMateria = repositorioMateria;
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
            return Result.Fail("Já existe uma questão com este enunciado para a mesma matéria.");
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
        }

        return Result.Ok();
    }

    public Result EditarRegistro(Guid id, Questao questaoEditada)
    {
        List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

        if (questoes.Any(q => q.Enunciado.Equals(questaoEditada.Enunciado)
        && q.Materia.Id.Equals(questaoEditada.Materia.Id) && q.Id != id))
        {
            return Result.Fail("Já existe uma questão com este enunciado para a mesma matéria.");
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
                return Result.Fail("Não é possível excluir a questão pois ela está vinculada a testes.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar excluir a questão.");
        }
    }

    public Result<Questao> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

            if (questaoSelecionada is null)
                return Result.Fail("Não foi possível obter o registro da questão selecionada.");

            return Result.Ok(questaoSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da questão {Id}.",
                id
                );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter a questão.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter as questões.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter as questões antigas.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter as questões não finalizadas.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar remover as questões.");
        }
    }

    public Result<Alternativa> SelecionarAlternativa(Questao questao, Guid idAlternativa)
    {
        try
        {
            Alternativa alternativaSelecionada = repositorioQuestao.SelecionarAlternativa(questao, idAlternativa)!;

            if (alternativaSelecionada is null)
                return Result.Fail("Não foi possível obter o registro da alternativa selecionada.");

            return Result.Ok(alternativaSelecionada);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção da alternativa {Id}.",
                idAlternativa
                );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter a alternativa.");
        }
    }

    public Result AdicionarAlternativa(Guid idQuestao, string textoAlternativa)
    {
        try
        {
            Questao questao = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (string.IsNullOrWhiteSpace(textoAlternativa))
                return Result.Fail("O texto da alternativa é obrigatório.");

            if (questao.Alternativas.Count >= 4)
                return Result.Fail("A questão já possui o máximo de 4 alternativas.");

            if (questao.Alternativas.Any(a => a.Texto.Equals(textoAlternativa)))
                return Result.Fail("Essa alternativa já foi inserida.");

            Alternativa novaAlternativa = new Alternativa(textoAlternativa, questao);

            questao.AderirAlternativa(novaAlternativa);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao adicionar alternativa à questão {Id}", idQuestao);

            return Result.Fail("Erro ao adicionar alternativa.");
        }
    }

    public Result RemoverAlternativa(Guid idQuestao, Guid idAlternativa)
    {
        try
        {
            Questao questao = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            Alternativa? alternativa = repositorioQuestao.SelecionarAlternativa(questao, idAlternativa)!;

            if (alternativa is null)
                return Result.Fail("Alternativa não encontrada.");

            questao.RemoverAlternativa(alternativa);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao remover alternativa da questão {Id}", idQuestao);

            return Result.Fail("Erro ao remover alternativa.");
        }
    }

    public Result MarcarAlternativaCorreta(Guid idQuestao, Guid idAlternativaCorreta)
    {
        try
        {
            Questao questao = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (questao == null)
                return Result.Fail("Questão não encontrada.");

            Alternativa? alternativa = questao.Alternativas.FirstOrDefault(a => a.Id == idAlternativaCorreta);

            if (alternativa is null)
                return Result.Fail("Alternativa não encontrada.");

            foreach (Alternativa a in questao.Alternativas)
                a.EstaCorreta = a.Id == idAlternativaCorreta;

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao marcar alternativa correta da questão {Id}", idQuestao);

            return Result.Fail("Erro ao marcar alternativa correta.");
        }
    }

    public Result FinalizarQuestao(Guid idQuestao)
    {
        try
        {
            Questao questao = repositorioQuestao.SelecionarRegistroPorId(idQuestao)!;

            if (questao == null)
                return Result.Fail("Questão não encontrada.");

            if (questao.Alternativas.Count < 2 || questao.Alternativas.Count > 4)
                return Result.Fail("A questão deve ter entre 2 e 4 alternativas.");

            if (questao.Alternativas.Count(a => a.EstaCorreta) != 1)
                return Result.Fail("A questão deve ter exatamente uma alternativa correta.");

            questao.Finalizado = true;

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao finalizar a questão {Id}", idQuestao);

            return Result.Fail("Erro ao finalizar a questão.");
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

            return Result.Fail("Ocorreu um erro durante a geração de questões da matéria");
        }
    }
}
