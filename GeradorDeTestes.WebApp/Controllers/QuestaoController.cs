using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("questoes")]
public class QuestaoController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    //private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioQuestao repositorioQuestao;

    public QuestaoController(GeradorDeTestesDbContext contexto, /*IRepositorioMateria repositorioMateria,*/ IRepositorioQuestao repositorioQuestao)
    {
        this.contexto = contexto;
        //this.repositorioMateria = repositorioMateria;
        this.repositorioQuestao = repositorioQuestao;
    }

    public IActionResult Index()
    {
        List<Questao> questoesNaoFinalizadas = repositorioQuestao.SelecionarNaoFinalizados();

        repositorioQuestao.RemoverRegistros(questoesNaoFinalizadas);

        List<Questao> questoes = repositorioQuestao.SelecionarRegistros()
                                                   .Where(q => q.Finalizado)
                                                   .ToList();

        VisualizarQuestoesViewModel visualizarVM = new(questoes);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        //List<Materia> materias = repositorioMateria.SelecionarRegistros();
        List<Materia> materias = contexto.Materias.ToList();

        CadastrarQuestaoViewModel cadastrarVM = new(materias);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarQuestaoViewModel cadastrarVM)
    {
        //Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(cadastrarVM.MateriaId)!;
        Materia? materia = contexto.Materias
                                .Include(m => m.Disciplina)
                                .FirstOrDefault(m => m.Id == cadastrarVM.MateriaId);

        Questao novaQuestao = cadastrarVM.ParaEntidade(materia!);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioQuestao.CadastrarRegistro(novaQuestao);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id = novaQuestao.Id });
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        //List<Materia> materias = repositorioMateria.SelecionarRegistros();
        List<Materia> materias = contexto.Materias.ToList();

        EditarQuestaoViewModel editarVM = new(
            questaoSelecionada,
            materias);

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    public IActionResult Editar(Guid id, EditarQuestaoViewModel editarVM)
    {
        //Materia materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(cadastrarVM.MateriaId)!;
        Materia materia = contexto.Materias
                                .Include(m => m.Disciplina)
                                .FirstOrDefault(m => m.Id == editarVM.MateriaId)!;

        Questao questaoEditada = editarVM.ParaEntidade(materia);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioQuestao.EditarRegistro(id, questaoEditada);

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

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        ExcluirQuestaoViewModel excluirVM = new(
            id,
            questaoSelecionada.Enunciado);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioQuestao.ExcluirRegistro(id);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }
        return RedirectToAction("Index");
    }

    [HttpGet, Route("/questoes/{id:guid}/detalhes")]
    public IActionResult Detalhes(Guid id)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        DetalhesQuestaoViewModel detalhesQuestaoVM = questaoSelecionada.ParaDetalhesVM();

        return View(detalhesQuestaoVM);
    }

    [HttpGet, Route("/questoes/{id:guid}/alternativas")]
    public IActionResult GerenciarAlternativas(Guid id)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        List<Alternativa> alternativas = questaoSelecionada.Alternativas.ToList();

        GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
            questaoSelecionada,
            alternativas);

        return View(gerenciarAlternativasVM);
    }

    [HttpPost, Route("/questoes/{id:guid}/adicionar-alternativa")]
    public IActionResult AdicionarAlternativa(Guid id, AdicionarAlternativaViewModel adicionarAlternativaVM)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        Alternativa novaAlternativa = new(
            adicionarAlternativaVM.TextoAlternativa,
            questaoSelecionada);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            questaoSelecionada.AderirAlternativa(novaAlternativa);

            contexto.Alternativas.Add(novaAlternativa);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost, Route("/questoes/{id:guid}/remover-alternativa/{idAlternativa:guid}")]
    public IActionResult RemoverAlternativa(Guid id, Guid idAlternativa)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;

        Alternativa alternativaEscolhida = repositorioQuestao.SelecionarAlternativa(questaoSelecionada, idAlternativa)!;

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            questaoSelecionada.RemoverAlternativa(alternativaEscolhida);

            contexto.Alternativas.Remove(alternativaEscolhida);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch (Exception)
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost("/questoes/{id:guid}/marcar-alternativa-correta")]
    public IActionResult MarcarAlternativaCorreta(Guid id, Guid idAlternativaCorreta)
    {
        Questao questao = repositorioQuestao.SelecionarRegistroPorId(id)!;

        foreach (Alternativa a in questao.Alternativas)
            a.EstaCorreta = (a.Id == idAlternativaCorreta);

        contexto.SaveChanges();

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost, Route("/questoes/{id:guid}/finalizar")]
    public IActionResult FinalizarQuestao(Guid id)
    {
        Questao questao = repositorioQuestao.SelecionarRegistroPorId(id)!;

        questao.Finalizado = true;

        contexto.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
