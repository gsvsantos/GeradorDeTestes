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
        //List<Materia> materias = repositorioMateria.SelecionarRegistros();

        Materia? materia = contexto.Materias
            .Include(m => m.Disciplina)
            .FirstOrDefault(m => m.Id == cadastrarVM.MateriaId);

        Questao novaQuestao = cadastrarVM.ParaEntidade(materia!);

        IDbContextTransaction transicao = contexto.Database.BeginTransaction();

        try
        {
            repositorioQuestao.CadastrarRegistro(novaQuestao);

            contexto.SaveChanges();

            transicao.Commit();
        }
        catch (Exception)
        {
            transicao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }
}
