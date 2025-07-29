using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
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
    private readonly ILogger<DisciplinaAppService> logger;

    public TesteAppService(IUnitOfWork unitOfWork, IRepositorioMateria repositorioMateria,
        IRepositorioQuestao repositorioQuestao, IRepositorioTeste repositorioTeste,
        ILogger<DisciplinaAppService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.repositorioMateria = repositorioMateria;
        this.repositorioQuestao = repositorioQuestao;
        this.repositorioTeste = repositorioTeste;
        this.logger = logger;
    }

    public Result CadastrarRegistro(Teste novoTeste)
    {
        List<Teste> testes = repositorioTeste.SelecionarRegistros();

        if (testes.Any(t => t.Titulo.Equals(novoTeste.Titulo)))
            return Result.Fail("Já existe um teste com este título.");

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
        }

        return Result.Ok();
    }

    public Result<Teste> DuplicarTeste(Guid id, string novoTitulo)
    {
        try
        {
            List<Teste> testes = repositorioTeste.SelecionarRegistros();

            if (testes.Any(t => t.Titulo.Equals(novoTitulo)))
                return Result.Fail("Já existe um teste com este título.");

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
            logger.LogError(ex, "Erro ao duplicar o teste {Id}", id);
            return Result.Fail("Erro ao duplicar o teste.");
        }
    }

    public Result AderirMateriaAoTeste(Guid id, Guid materiaId)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            Materia materia = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materia is null)
                return Result.Fail("Não foi possível obter o registro da matéria selecionada.");

            testeSelecionado.AderirMateria(materia);

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao adicionar matéria {MateriaId} ao teste {Id}", materiaId, id);

            return Result.Fail("Erro ao adicionar matéria ao teste.");
        }
    }

    public Result RemoverMateriaDoTeste(Guid id, Guid materiaId)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            Materia materia = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materia is null)
                return Result.Fail("Não foi possível obter o registro da matéria selecionada.");

            testeSelecionado.RemoverMateria(materia);
            testeSelecionado.Questoes.RemoveAll(q => q.Materia.Id == materia.Id);
            testeSelecionado.QuantidadesPorMateria.RemoveAll(qpm => qpm.Materia.Id == materia.Id);

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao remover matéria {MateriaId} do teste {Id}", materiaId, id);

            return Result.Fail("Erro ao remover matéria do teste.");
        }
    }

    public Result DefinirQuantidadeQuestoesPorMateria(Guid id, Guid materiaId, int novaQuantidade)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(materiaId)!;

            if (materiaSelecionada is null)
                return Result.Fail("Não foi possível obter o registro da matéria selecionada.");

            TesteMateriaQuantidade? objComQuantidade = testeSelecionado.QuantidadesPorMateria
                .FirstOrDefault(x => x.Materia.Id.Equals(materiaId));

            int quantidadeAnterior = objComQuantidade?.QuantidadeQuestoes ?? 0;

            int quantidadeTotalQuestoes = testeSelecionado.QuantidadesPorMateria.Sum(q => q.QuantidadeQuestoes);

            int novaQuantidadeTotal = quantidadeTotalQuestoes - quantidadeAnterior + novaQuantidade;

            if (novaQuantidade > materiaSelecionada.Questoes.Count)
                return Result.Fail("A quantidade inserida é maior do que a quantidade de questões da matéria.");

            if (novaQuantidadeTotal > testeSelecionado.QuantidadeQuestoes)
                return Result.Fail($"A quantidade inserida ultrapassa o limite de questões do teste! Atual: {quantidadeTotalQuestoes}/{testeSelecionado.QuantidadeQuestoes}");

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

            logger.LogError(ex,
                "Erro ao definir quantidade de questões para a matéria {MateriaId} no teste {TesteId}",
                materiaId,
                id);

            return Result.Fail("Erro ao definir quantidade de questões.");
        }
    }

    public Result GerarQuestoesParaTeste(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

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

            logger.LogError(ex,
                "Erro ao gerar questões para o teste {Id}",
                id);

            return Result.Fail("Erro ao gerar questões.");
        }
    }

    public Result<Teste> GerarQuestoesParaProvao(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            List<Materia> materias = repositorioMateria.SelecionarRegistros()
                .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id && m.Serie == testeSelecionado.Serie)
                .ToList();

            materias.Shuffle();

            testeSelecionado.Materias.Clear();
            testeSelecionado.Questoes.Clear();
            testeSelecionado.QuantidadesPorMateria.Clear();

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
            return Result.Fail(new Error("Erro ao gerar provão: " + ex.Message));
        }
    }

    public Result AtualizarQuestoes(Guid id)
    {
        Result<Teste> testeResult = SelecionarRegistroPorId(id);

        if (testeResult.IsFailed)
            return Result.Fail(testeResult.Errors.First().Message);

        Teste teste = testeResult.Value;

        if (teste.EhProvao)
            return GerarQuestoesAutomaticamente(id);

        if (teste.Questoes.Count == teste.QuantidadeQuestoes)
            return EmbaralharQuestoes(id);

        return GerarQuestoesAutomaticamente(id);
    }

    public Result GerarQuestoesAutomaticamente(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            testeSelecionado.Questoes.Clear();
            testeSelecionado.QuantidadesPorMateria.Clear();

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

            foreach (Materia materia in materias)
            {
                List<Questao> questoesDaMateria = questoesSelecionadas
                    .Where(q => q.Materia.Id == materia.Id)
                    .ToList();

                if (questoesDaMateria.Count == 0)
                    continue;

                testeSelecionado.AderirMateria(materia);

                foreach (Questao questao in questoesDaMateria)
                {
                    testeSelecionado.AderirQuestao(questao);
                    repositorioTeste.AtualizarQuantidadePorMateria(testeSelecionado, materia);
                }
            }

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

            return Result.Fail("Erro ao gerar questões automaticamente.");
        }
    }

    public Result EmbaralharQuestoes(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            testeSelecionado.Materias.Clear();
            testeSelecionado.Questoes.Clear();
            testeSelecionado.QuantidadesPorMateria.Clear();

            List<Materia> materias = repositorioMateria.SelecionarRegistros()
                .Where(m => m.Disciplina.Id == testeSelecionado.Disciplina.Id &&
                            m.Serie == testeSelecionado.Serie)
                .ToList();

            materias.Shuffle();

            List<Questao> todasQuestoes = repositorioQuestao.SelecionarRegistros()
                .Where(q => materias.Select(m => m.Id).Contains(q.Materia.Id) && q.Finalizado)
                .ToList();

            todasQuestoes.Shuffle();

            List<Questao> questoesSelecionadas = todasQuestoes
                .DistinctBy(q => q.Id)
                .Take(testeSelecionado.QuantidadeQuestoes)
                .ToList();

            foreach (Materia materia in materias)
            {
                List<Questao> questoesDaMateria = questoesSelecionadas
                    .Where(q => q.Materia.Id == materia.Id)
                    .ToList();

                if (questoesDaMateria.Count == 0)
                    continue;

                testeSelecionado.AderirMateria(materia);

                foreach (Questao questao in questoesDaMateria)
                {
                    testeSelecionado.AderirQuestao(questao);
                    repositorioTeste.AtualizarQuantidadePorMateria(testeSelecionado, materia);
                }
            }

            testeSelecionado.Questoes.Shuffle();

            RemoverMateriasSemQuestoes(testeSelecionado);

            repositorioTeste.AtualizarRegistro(testeSelecionado);
            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao embaralhar questões para o teste {Id}",
                id);

            return Result.Fail("Erro ao embaralhar questões.");
        }
    }

    public Result ExcluirRegistro(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            repositorioTeste.ExcluirRegistro(id);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex,
                "Erro ao excluir o teste {Id}",
                id);

            return Result.Fail("Erro ao excluir o teste.");
        }
    }

    public Result<Teste> SelecionarRegistroPorId(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            return Result.Ok(testeSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do teste {Id}.",
                id
                );

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter o teste.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter os testes.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter os testes antigos.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar obter os testes não finalizados.");
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

            return Result.Fail("Ocorreu um erro inesperado ao tentar remover os testes.");
        }
    }

    public Result AtualizarQuestoesETeste(Teste testeSelecionado)
    {
        try
        {
            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao atualizar questões do teste {Id}", testeSelecionado.Id);

            return Result.Fail("Erro ao atualizar as questões do teste.");
        }
    }

    public Result LimparQuestoesEQuantidades(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            testeSelecionado.Questoes.Clear();
            testeSelecionado.QuantidadesPorMateria.Clear();

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            logger.LogError(ex, "Erro ao limpar questões e quantidades do teste {Id}", id);
            return Result.Fail("Erro ao limpar dados do teste.");
        }
    }

    public Result FinalizarTeste(Guid id)
    {
        try
        {
            Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

            if (testeSelecionado is null)
                return Result.Fail("Não foi possível obter o registro do teste selecionado.");

            if (testeSelecionado.Questoes.Count < testeSelecionado.QuantidadeQuestoes)
                return Result.Fail("O teste ainda não tem todas as questões necessárias.");

            testeSelecionado.Finalizado = true;

            repositorioTeste.AtualizarRegistro(testeSelecionado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            logger.LogError(ex, "Erro ao finalizar o teste {Id}", id);
            return Result.Fail("Erro ao finalizar o teste.");
        }
    }

    private void RemoverMateriasSemQuestoes(Teste teste)
    {
        HashSet<Guid> idsMateriasComQuestoes = teste.Questoes
            .Select(q => q.Materia.Id)
            .Distinct()
            .ToHashSet();

        // Remove matérias que não têm nenhuma questão associada
        teste.Materias.RemoveAll(m => !idsMateriasComQuestoes.Contains(m.Id));
        teste.QuantidadesPorMateria.RemoveAll(qpm => !idsMateriasComQuestoes.Contains(qpm.Materia.Id));
    }
}
