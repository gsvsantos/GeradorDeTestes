using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

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
}
