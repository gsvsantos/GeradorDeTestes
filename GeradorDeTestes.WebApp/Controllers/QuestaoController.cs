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
        List<Questao> questoes = repositorioQuestao.SelecionarRegistros();

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

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        Questao questaoSelecionada = repositorioQuestao.SelecionarRegistroPorId(id)!;
        //List<Materia> materias = repositorioMateria.SelecionarRegistros();
        List<Materia> materias = contexto.Materias.ToList();

        EditarQuestaoViewModel editarVM = new(questaoSelecionada, materias);

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
        Questao questao = repositorioQuestao.SelecionarRegistroPorId(id)!;

        ExcluirQuestaoViewModel excluirVM = new(
            id,
            questao.Enunciado);

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
}
