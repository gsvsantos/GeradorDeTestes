using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("testes")]
public class TesteController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    private readonly IRepositorioTeste repositorioTeste;

    public TesteController(GeradorDeTestesDbContext contexto, IRepositorioTeste repositorioTeste)
    {
        this.contexto = contexto;
        this.repositorioTeste = repositorioTeste;
    }

    public IActionResult Index()
    {
        List<Teste> testes = repositorioTeste.SelecionarRegistros();

        VisualizarTestesViewModel visualizarVM = new(testes);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        List<Disciplina> disciplinas = contexto.Disciplinas.ToList();

        CadastrarTesteViewModel cadastrarVM = new(disciplinas);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarTesteViewModel cadastrarVM)
    {
        Disciplina? disciplina = contexto.Disciplinas.FirstOrDefault(d => d.Id.Equals(cadastrarVM.DisciplinaId))!;

        Teste novoTeste = cadastrarVM.ParaEntidade(disciplina);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioTeste.CadastrarRegistro(novoTeste);
            contexto.Testes.Add(novoTeste);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        string tipoGeracao = cadastrarVM.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id = novoTeste.Id });
    }

    [HttpGet("gerar-teste")]
    public IActionResult GerarTeste(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        List<Materia> materiasSelecionadas = testeSelecionado.Materias;

        List<Materia> materias = contexto.Materias.Where(m => m.Disciplina.Equals(testeSelecionado.Disciplina))
            .Where(m => m.Serie.Equals(testeSelecionado.Serie))
            .ToList();

        testeSelecionado.Questoes.Clear();

        foreach (TesteMateriaQuantidade q in testeSelecionado.QuantidadesPorMateria)
        {
            List<Questao>? questoesDaMateria = contexto.Questoes
                .Where(questao => questao.Materia.Id.Equals(q.Materia.Id))
                .Take(q.QuantidadeQuestoes)
                .ToList();

            foreach (Questao questao in questoesDaMateria)
            {
                testeSelecionado.AderirQuestao(questao);
            }
        }

        if (TempData["Embaralhar"] is not null)
        {
            if (testeSelecionado.Questoes.Count < testeSelecionado.QuantidadeQuestoes)
            {
                List<Questao> todasQuestoes = new List<Questao>();

                foreach (Materia materia in materiasSelecionadas)
                {
                    List<Questao> questoesDaMateria = contexto.Questoes
                        .Where(q => q.Materia.Id == materia.Id)
                        .Take(testeSelecionado.QuantidadeQuestoes)
                        .ToList();

                    todasQuestoes.AddRange(questoesDaMateria);
                }

                todasQuestoes.Shuffle();

                foreach (Questao questao in todasQuestoes.Take(testeSelecionado.QuantidadeQuestoes - testeSelecionado.Questoes.Count).ToList())
                {
                    testeSelecionado.AderirQuestao(questao);
                    repositorioTeste.AtualizarQuantidadePorMateria(testeSelecionado, questao.Materia);
                }
            }

            testeSelecionado.Questoes.Shuffle();
        }

        GerarTesteViewModel gerarTesteVM = testeSelecionado.ParaGerarTesteVM(materias, materiasSelecionadas);

        return View(gerarTesteVM);
    }

    [HttpPost("gerar-teste")]
    public IActionResult GerarTeste(Guid id, GerarTestePostViewModel gerarTestePostVM)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            contexto.Update(testeSelecionado);
            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, Route("/testes/{id:guid}/selecionar-materia/{materiaId:guid}")]
    public IActionResult SelecionarMateria(Guid id, Guid materiaId)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        Materia materiaSelecionada = contexto.Materias
                                .Include(m => m.Disciplina)
                                .FirstOrDefault(m => m.Id.Equals(materiaId))!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            testeSelecionado.AderirMateria(materiaSelecionada);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpPost, Route("/testes/{id:guid}/remover-materia/{materiaId:guid}")]
    public IActionResult RemoverMateria(Guid id, Guid materiaId)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        Materia materiaSelecionada = contexto.Materias
                                .Include(m => m.Disciplina)
                                .FirstOrDefault(m => m.Id.Equals(materiaId))!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            testeSelecionado.RemoverMateria(materiaSelecionada);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpGet, Route("/testes/{id:guid}/definir-quantidade-questoes/{materiaId:guid}")]
    public IActionResult DefinirQuantidadeQuestoes(Guid id, Guid materiaId)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        Materia materiaSelecionada = contexto.Materias.FirstOrDefault(m => m.Id.Equals(materiaId))!;

        List<Questao> questoes = materiaSelecionada.Questoes.ToList();

        Disciplina disciplina = contexto.Disciplinas
                        .Include(m => m.Materias)
                        .Include(m => m.Testes)
                        .FirstOrDefault(m => m.Id == testeSelecionado.Disciplina.Id)!;

        List<Materia> materias = contexto.Materias.Where(m => m.Disciplina.Equals(disciplina))
            .Where(m => m.Serie.Equals(testeSelecionado.Serie))
            .ToList();

        DefinirQuantidadeQuestoesViewModel vm = new()
        {
            Id = testeSelecionado.Id,
            Titulo = testeSelecionado.Titulo,
            NomeDisciplina = disciplina.Nome,
            Serie = testeSelecionado.Serie,
            MateriaId = materiaId,
            Questoes = questoes.Select(q => new SelectListItem()
            {
                Text = q.Enunciado,
                Value = q.Id.ToString()
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost, Route("/testes/{id:guid}/definir-quantidade-questoes/{materiaId:guid}")]
    public IActionResult DefinirQuantidadeQuestoes(Guid id, Guid materiaId, DefinirQuantidadeQuestoesPostViewModel vm)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        Materia materiaSelecionada = testeSelecionado.Materias.FirstOrDefault(m => m.Id.Equals(materiaId))!;

        TesteMateriaQuantidade? objComQuantidade = testeSelecionado.QuantidadesPorMateria
            .FirstOrDefault(x => x.Materia.Id == materiaId);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            if (vm.QuantidadeQuestoesMateria == 0 && objComQuantidade is not null)
            {
                testeSelecionado.QuantidadesPorMateria.Remove(objComQuantidade);
                contexto.QuantidadesPorMateria.Remove(objComQuantidade!);
            }
            else if (objComQuantidade is not null)
            {
                objComQuantidade.QuantidadeQuestoes = vm.QuantidadeQuestoesMateria;
            }
            else
            {
                objComQuantidade = new()
                {
                    Id = Guid.NewGuid(),
                    Materia = materiaSelecionada,
                    QuantidadeQuestoes = vm.QuantidadeQuestoesMateria
                };

                testeSelecionado.QuantidadesPorMateria.Add(objComQuantidade);
                contexto.QuantidadesPorMateria.Add(objComQuantidade!);
            }

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpPost, Route("/testes/{id:guid}/aleatorizar-questoes")]
    public IActionResult AleatorizarQuestoes(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        TempData["Embaralhar"] = true;

        string tipoGeracao = testeSelecionado.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id });
    }

    [HttpGet("gerar-provao")]
    public IActionResult GerarProvao(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        List<Materia> materias = contexto.Materias.Where(m => m.Disciplina.Equals(testeSelecionado.Disciplina))
            .Where(m => m.Serie.Equals(testeSelecionado.Serie))
            .ToList();

        materias.Shuffle();

        List<Materia> materiasSelecionadas = materias;

        testeSelecionado.Questoes.Clear();
        testeSelecionado.QuantidadesPorMateria.Clear();

        if (testeSelecionado.Questoes.Count < testeSelecionado.QuantidadeQuestoes)
        {
            List<Questao> todasQuestoes = new List<Questao>();

            foreach (Materia materia in materiasSelecionadas)
            {
                List<Questao> questoesDaMateria = contexto.Questoes
                    .Where(q => q.Materia.Id == materia.Id)
                    .Take(testeSelecionado.QuantidadeQuestoes)
                    .ToList();

                todasQuestoes.AddRange(questoesDaMateria);
                testeSelecionado.AderirMateria(materia);
            }

            todasQuestoes.Shuffle();

            foreach (Questao questao in todasQuestoes.Take(testeSelecionado.QuantidadeQuestoes - testeSelecionado.Questoes.Count).ToList())
            {
                testeSelecionado.AderirQuestao(questao);
                repositorioTeste.AtualizarQuantidadePorMateria(testeSelecionado, questao.Materia);
            }
        }

        testeSelecionado.Questoes.Shuffle();

        GerarProvaoViewModel gerarProvaoVM = testeSelecionado.ParaGerarProvaoVM(materias, materiasSelecionadas);

        return View(gerarProvaoVM);
    }

    [HttpPost("gerar-provao")]
    public IActionResult GerarProvao(Guid id, GerarProvaoPostViewModel gerarProvaoPostVM)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            contexto.Update(testeSelecionado);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("duplicar/{id:guid}")]
    public IActionResult Duplicar(Guid id)
    {
        DuplicarViewModel duplicarVM = new();

        return View(duplicarVM);
    }

    [HttpPost("duplicar/{id:guid}")]
    public IActionResult Duplicar(Guid id, DuplicarViewModel duplicarVM)
    {
        Teste testeOriginal = repositorioTeste.SelecionarRegistroPorId(id)!;

        Teste novoTeste = new Teste()
        {
            Id = Guid.NewGuid(),
            Titulo = duplicarVM.Titulo,
            Disciplina = testeOriginal.Disciplina,
            Serie = testeOriginal.Serie,
            EhProvao = testeOriginal.EhProvao,
            QuantidadeQuestoes = testeOriginal.QuantidadeQuestoes,
            Materias = testeOriginal.Materias.ToList(),
            Questoes = new List<Questao>(),
            QuantidadesPorMateria = new List<TesteMateriaQuantidade>()
        };

        foreach (TesteMateriaQuantidade qpm in testeOriginal.QuantidadesPorMateria)
        {
            novoTeste.QuantidadesPorMateria.Add(new TesteMateriaQuantidade
            {
                Id = Guid.NewGuid(),
                Materia = qpm.Materia,
                QuantidadeQuestoes = qpm.QuantidadeQuestoes
            });
        }

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            contexto.Testes.Add(novoTeste);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();

            throw;
        }

        string tipoGeracao = novoTeste.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id = novoTeste.Id });
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        ExcluirTesteViewModel excluirVM = new ExcluirTesteViewModel(id, testeSelecionado.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioTeste.ExcluirRegistro(id);
            contexto.Testes.Remove(testeSelecionado);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/testes/{id:guid}/detalhes-teste")]
    public IActionResult DetalhesTeste(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        DetalhesTesteViewModel detalhesTesteVM = testeSelecionado.ParaDetalhesTesteVM();

        return View(detalhesTesteVM);
    }

    [HttpGet, Route("/testes/{id:guid}/detalhes-provao")]
    public IActionResult DetalhesProvao(Guid id)
    {
        Teste testeSelecionado = repositorioTeste.SelecionarRegistroPorId(id)!;

        DetalhesProvaoViewModel detalhesProvaoVM = testeSelecionado.ParaDetalhesProvaoVM();

        return View(detalhesProvaoVM);
    }
}
