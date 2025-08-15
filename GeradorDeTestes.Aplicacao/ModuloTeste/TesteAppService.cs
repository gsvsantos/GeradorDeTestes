using FluentResults;
using GeradorDeTestes.Aplicacao.Compartilhado;
using GeradorDeTestes.Dominio.Compartilhado;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.Extensions.Logging;

namespace GeradorDeTestes.Aplicacao.ModuloTeste;
public class TesteAppService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioQuestao repositorioQuestao;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly ILogger<TesteAppService> logger;

    public TesteAppService(IUnitOfWork unitOfWork, IRepositorioMateria repositorioMateria,
        IRepositorioQuestao repositorioQuestao, IRepositorioTeste repositorioTeste,
        ILogger<TesteAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioMateria = repositorioMateria;
        this.repositorioQuestao = repositorioQuestao;
        this.repositorioTeste = repositorioTeste;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Teste novoTeste)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros();
        List<Teste> testes = repositorioTeste.SelecionarRegistros();

        if (testes.Any(t => t.Titulo.Equals(novoTeste.Titulo)))
        {
            Error erro = ResultadosErro.RegistroDuplicadoErro(
                "Já existe um teste com este título.");

            return Result.Fail(erro);
        }

        List<Materia> materiasSelecionadas = materias
            .Where(m => m.Disciplina.Equals(novoTeste.Disciplina) && m.Serie.Equals(novoTeste.Serie))
            .ToList();

        if (materiasSelecionadas.Count == 0)
        {
            Error erro = ResultadosErro.DisciplinaESerieSemMateriasErro(
                "A disciplina/série selecionadas não contêm matérias.");

            return Result.Fail(erro);
        }

        if (materiasSelecionadas.All(m => m.Questoes == null || m.Questoes.Count < 1))
        {
            Error erro = ResultadosErro.MateriaSemQuestoesErro(
                "Essa disciplina/série não possui matérias com questões.");

            return Result.Fail(erro);
        }

        try
        {
            repositorioTeste.CadastrarRegistro(novoTeste);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o cadastro de {@ViewModel}.",
                novoTeste
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }

        return Result.Ok();
    }

    public Result<Teste> DuplicarTeste(Guid id, string novoTitulo)
    {
        try
        {
            List<Teste> testes = repositorioTeste.SelecionarRegistros();

            if (testes.Any(t => t.Titulo.Equals(novoTitulo)))
            {
                Error erro = ResultadosErro.RegistroDuplicadoErro(
                    "Já existe um teste com este título.");

                return Result.Fail(erro);
            }

            Teste testeOriginal = repositorioTeste.SelecionarRegistroPorId(id)!;

            Teste novoTeste = new Teste()
            {
                Id = Guid.NewGuid(),
                Titulo = novoTitulo,
                Disciplina = testeOriginal.Disciplina,
                Serie = testeOriginal.Serie,
                EhProvao = testeOriginal.EhProvao,
                QuantidadeQuestoes = testeOriginal.QuantidadeQuestoes,
                Materias = testeOriginal.Materias.ToList(),
                Questoes = new List<Questao>(),
                QuantidadesPorMateria = testeOriginal.QuantidadesPorMateria
                .Select(qpm => new TesteMateriaQuantidade
                {
                    Id = Guid.NewGuid(),
                    Materia = qpm.Materia,
                    QuantidadeQuestoes = qpm.QuantidadeQuestoes
                }).ToList()
            };

            repositorioTeste.CadastrarRegistro(novoTeste);

            unitOfWork.Commit();

            return Result.Ok(novoTeste);
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao duplicar o teste {Id}",
                id
                );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result AderirMateriaAoTeste(Guid id, Guid materiaId)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materiaSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(materiaId);

                return Result.Fail(erro);
            }

            testeSelecionado.AderirMateria(materiaSelecionada);

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao adicionar matéria {MateriaId} ao teste {Id}",
                materiaId, id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result RemoverMateriaDoTeste(Guid id, Guid materiaId)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materiaSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(materiaId);

                return Result.Fail(erro);
            }

            testeSelecionado.RemoverMateria(materiaSelecionada);
            testeSelecionado.Questoes.RemoveAll(q => q.Materia.Id == materiaSelecionada.Id);
            testeSelecionado.QuantidadesPorMateria.RemoveAll(qpm => qpm.Materia.Id == materiaSelecionada.Id);

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao remover matéria {MateriaId} do teste {Id}",
                materiaId, id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result DefinirQuantidadeQuestoesPorMateria(Guid id, Guid materiaId, int novaQuantidade)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materiaSelecionada is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(materiaId);

                return Result.Fail(erro);
            }

            TesteMateriaQuantidade? objComQuantidade = testeSelecionado.QuantidadesPorMateria
                .FirstOrDefault(x => x.Materia.Id.Equals(materiaId));

            int quantidadeAnterior = objComQuantidade?.QuantidadeQuestoes ?? 0;

            int quantidadeTotalQuestoes = testeSelecionado.QuantidadesPorMateria.Sum(q => q.QuantidadeQuestoes);

            int novaQuantidadeTotal = quantidadeTotalQuestoes - quantidadeAnterior + novaQuantidade;

            if (novaQuantidade > materiaSelecionada.Questoes.Count)
            {
                Error erro = ResultadosErro.QuantidadeQuestoesErro(
                    "A quantidade inserida é maior do que a quantidade de questões da matéria.");

                return Result.Fail(erro);
            }
            else if (novaQuantidadeTotal > testeSelecionado.QuantidadeQuestoes)
            {
                Error erro = ResultadosErro.QuantidadeQuestoesErro(
                    $"A quantidade inserida ultrapassa o limite de questões do teste! Atual: {quantidadeTotalQuestoes}/{testeSelecionado.QuantidadeQuestoes}");

                return Result.Fail(erro);
            }

            if (novaQuantidade == 0 && objComQuantidade is not null)
            {
                testeSelecionado.QuantidadesPorMateria.Remove(objComQuantidade);
                testeSelecionado.Materias.RemoveAll(m => m.Id == materiaSelecionada.Id);
                testeSelecionado.Questoes.RemoveAll(q => q.Materia.Id == materiaSelecionada.Id);
            }
            else if (objComQuantidade is not null)
            {
                objComQuantidade.QuantidadeQuestoes = novaQuantidade;
            }
            else
            {
                objComQuantidade = new()
                {
                    Id = Guid.NewGuid(),
                    Materia = materiaSelecionada,
                    QuantidadeQuestoes = novaQuantidade
                };

                testeSelecionado.QuantidadesPorMateria.Add(objComQuantidade);
            }

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao definir quantidade de questões para a matéria {MateriaId} no teste {TesteId}",
                materiaId,
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result GerarQuestoesParaTeste(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            List<Guid> materiasSelecionadas = testeSelecionado.QuantidadesPorMateria
                .Where(q => q.QuantidadeQuestoes > 0)
                .Select(q => q.Materia.Id)
                .ToList();

            List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
                .Where(q => materiasSelecionadas.Contains(q.Materia.Id) && q.Finalizado)
                .ToList();

            testeSelecionado.Questoes.Clear();

            foreach (TesteMateriaQuantidade qpm in testeSelecionado.QuantidadesPorMateria)
            {
                if (qpm.QuantidadeQuestoes == 0)
                    continue;

                List<Questao> questoesDaMateria = todasQuestoes
                    .Where(q => q.Materia.Id == qpm.Materia.Id)
                    .Take(qpm.QuantidadeQuestoes)
                    .ToList();

                foreach (Questao questao in questoesDaMateria)
                    testeSelecionado.AderirQuestao(questao);
            }

            testeSelecionado.Questoes.Shuffle();

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao gerar questões para o teste {Id}",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Teste> GerarQuestoesParaProvao(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            List<Materia> materias = repositorioMateria.SelecionarRegistros()
                .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id && m.Serie == testeSelecionado.Serie)
                .ToList();

            materias.Shuffle();

            testeSelecionado.Materias.Clear();
            testeSelecionado.Questoes.Clear();
            LimparQuantidadesPorMateria(testeSelecionado);

            List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
                .Where(q => materias.Select(m => m.Id).Contains(q.Materia.Id) && q.Finalizado)
                .ToList();

            todasQuestoes.Shuffle();

            foreach (Questao questao in todasQuestoes.Take(testeSelecionado.QuantidadeQuestoes))
            {
                if (testeSelecionado.Questoes.Any(q => q.Equals(questao)))
                    continue;

                Materia materia = questao.Materia;

                if (!testeSelecionado.Materias.Any(m => m.Id == materia.Id))
                    testeSelecionado.AderirMateria(materia);

                testeSelecionado.AderirQuestao(questao);
                repositorioTeste.AtualizarQuantidadePorMateria(testeSelecionado, materia);
            }

            testeSelecionado.Questoes.Shuffle();

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok(testeSelecionado);
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao gerar provão {Message}",
                ex.Message
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result AtualizarQuestoes(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        if (testeSelecionado is null)
        {
            Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

            return Result.Fail(erro);
        }

        if (testeSelecionado.EhProvao)
            return GerarQuestoesAutomaticamente(id);

        if (testeSelecionado.Questoes.Count != 0)
            return EmbaralharQuestoes(id);

        return GerarQuestoesAutomaticamente(id);
    }

    public Result GerarQuestoesAutomaticamente(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            LimparQuestoesEQuantidades(testeSelecionado);

            List<Materia> materias = repositorioMateria.SelecionarRegistros()
                .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id &&
                            m.Serie == testeSelecionado.Serie)
                .ToList();

            if (testeSelecionado.EhProvao)
                materias.Shuffle();

            List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
                .Where(q => materias.Select(m => m.Id).Contains(q.Materia.Id) && q.Finalizado)
                .ToList();

            todasQuestoes.Shuffle();

            List<Questao> questoesSelecionadas = todasQuestoes
                .DistinctBy(q => q.Id)
                .Take(testeSelecionado.QuantidadeQuestoes)
                .ToList();

            AdicionarQuestoesAoTeste(testeSelecionado, questoesSelecionadas);

            testeSelecionado.Questoes.Shuffle();

            repositorioTeste.AtualizarRegistro(testeSelecionado);
            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao gerar questões automaticamente para o teste {Id}",
                id);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result EmbaralharQuestoes(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            if (testeSelecionado.EhProvao)
                return EmbaralharQuestoesParaProvao(testeSelecionado);

            return EmbaralharQuestoesParaTeste(testeSelecionado);
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao embaralhar questões para o teste {Id}",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result ExcluirRegistro(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            repositorioTeste.ExcluirRegistro(id);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Erro ao excluir o teste {Id}",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Teste> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            return Result.Ok(testeSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do teste {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Teste>> SelecionarRegistros()
    {
        try
        {
            List<Teste> testes = repositorioTeste.SelecionarRegistros();

            return Result.Ok(testes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção dos testes registrados."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Teste>> SelecionarNaoFinalizadosAntigos(TimeSpan tempoMaximo)
    {
        try
        {
            List<Teste> testes = repositorioTeste.SelecionarNaoFinalizadosAntigos(tempoMaximo);

            return Result.Ok(testes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção dos testes antigos não finalizados."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Teste>> SelecionarNaoFinalizados()
    {
        try
        {
            List<Teste> testes = repositorioTeste.SelecionarNaoFinalizados();

            return Result.Ok(testes);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção dos testes não finalizados."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Teste>> RemoverRegistros(List<Teste> testes)
    {
        try
        {
            repositorioTeste.RemoverRegistros(testes);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a remoção dos testes."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result FinalizarTeste(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
            {
                Error erro = ResultadosErro.RegistroNaoEncontradoErro(id);

                return Result.Fail(erro);
            }

            if (testeSelecionado.Questoes.Count < testeSelecionado.QuantidadeQuestoes)
            {
                Error erro = ResultadosErro.QuantidadeQuestoesErro(
                    "O teste ainda não tem todas as questões necessárias.");

                return Result.Fail(erro);
            }

            testeSelecionado.Finalizado = true;

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            logger.LogError(
                ex,
                "Erro ao finalizar o teste {Id}",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    private Result EmbaralharQuestoesParaTeste(Teste testeSelecionado)
    {
        List<Materia> materias = repositorioMateria.SelecionarRegistros()
            .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id &&
                        m.Serie == testeSelecionado.Serie)
            .ToList();

        List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
            .Where(q => materias.Select(m => m.Id).Contains(q.Materia.Id) && q.Finalizado)
            .ToList();

        List<Questao> questoesSelecionadas = todasQuestoes
            .DistinctBy(q => q.Id)
            .Take(testeSelecionado.QuantidadesPorMateria.Count)
            .ToList();

        AdicionarQuestoesAoTeste(testeSelecionado, questoesSelecionadas);

        testeSelecionado.Questoes.Shuffle();

        RemoverMateriasSemQuestoes(testeSelecionado);

        repositorioTeste.AtualizarRegistro(testeSelecionado);
        unitOfWork.Commit();

        return Result.Ok();
    }

    private Result EmbaralharQuestoesParaProvao(Teste testeSelecionado)
    {
        testeSelecionado.Materias.Clear();
        LimparQuestoesEQuantidades(testeSelecionado);

        List<Materia> materias = repositorioMateria.SelecionarRegistros()
            .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id &&
                        m.Serie == testeSelecionado.Serie)
            .ToList();

        List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
            .Where(q => materias.Select(m => m.Id).Contains(q.Materia.Id) && q.Finalizado)
            .ToList();

        todasQuestoes.Shuffle();

        List<Questao> questoesSelecionadas = todasQuestoes
            .DistinctBy(q => q.Id)
            .Take(testeSelecionado.QuantidadeQuestoes)
            .ToList();

        AdicionarQuestoesAoTeste(testeSelecionado, questoesSelecionadas);

        testeSelecionado.Questoes.Shuffle();

        repositorioTeste.AtualizarRegistro(testeSelecionado);
        unitOfWork.Commit();

        return Result.Ok();
    }

    private void LimparQuantidadesPorMateria(Teste testeSelecionado)
    {
        foreach (TesteMateriaQuantidade? quantidade in testeSelecionado.QuantidadesPorMateria.ToList())
            repositorioTeste.RemoverQuantidadePorMateria(quantidade);

        testeSelecionado.QuantidadesPorMateria.Clear();
    }

    private void LimparQuestoesEQuantidades(Teste testeSelecionado)
    {
        foreach (TesteMateriaQuantidade quantidade in testeSelecionado.QuantidadesPorMateria.ToList())
            repositorioTeste.RemoverQuantidadePorMateria(quantidade);

        testeSelecionado.QuantidadesPorMateria.Clear();
        testeSelecionado.Questoes.Clear();
    }

    private void RemoverMateriasSemQuestoes(Teste teste)
    {
        HashSet<Guid> idsMateriasComQuestoes = teste.Questoes
            .Select(q => q.Materia.Id)
            .Distinct()
            .ToHashSet();

        teste.Materias.RemoveAll(m => !idsMateriasComQuestoes.Contains(m.Id));
        teste.QuantidadesPorMateria.RemoveAll(qpm => !idsMateriasComQuestoes.Contains(qpm.Materia.Id));
    }

    private void AdicionarQuestoesAoTeste(Teste teste, List<Questao> questoesSelecionadas)
    {
        List<IGrouping<Materia, Questao>> questoesPorMateria = questoesSelecionadas
            .GroupBy(q => q.Materia)
            .ToList();

        foreach (IGrouping<Materia, Questao>? grupo in questoesPorMateria)
        {
            Materia materia = grupo.Key;

            if (!grupo.Any())
                continue;

            if (!teste.Materias.Any(m => m.Id == materia.Id))
                teste.AderirMateria(materia);

            foreach (Questao? questao in grupo)
            {
                if (teste.Questoes.Any(q => q.Id == questao.Id))
                    continue;

                teste.AderirQuestao(questao);
                repositorioTeste.AtualizarQuantidadePorMateria(teste, materia);
            }
        }

        teste.Questoes.Shuffle();

        RemoverMateriasSemQuestoes(teste);
    }
}
