using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Microsoft.AspNetCore.Components.Route("materias")]
public class MateriaController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    private readonly IRepositorioMateria repositorioMateria;
    //private readonly IRepositorioDisciplina repositorioDisciplina;
    public MateriaController(GeradorDeTestesDbContext contexto, IRepositorioMateria repoitorioMateria, IRepositorioDisciplina repositorioDisciplina)
    {
        this.contexto = contexto;
        this.repositorioMateria = repoitorioMateria;
        //this.repositorioDisciplina = repositorioDisciplina;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var registros = repositorioMateria.SelecionarRegistros();
        var visualizarVM = new VisualizarMateriaViewModel(registros);
        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        //List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

        List <Disciplina> disciplinas = contexto.Disciplinas.ToList();

        CadastrarMateriaViewModel cadastrarVM = new CadastrarMateriaViewModel(disciplinas);
        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarMateriaViewModel cadastrarVM)
    {
        //Disciplina? disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(cadastrarVM.DisciplinaId);

        Disciplina? disciplina = contexto.Disciplinas
            .Include(disc => disc.Materias)
            .Include(disc => disc.Testes)
            .FirstOrDefault(disc => disc.Id.Equals(cadastrarVM.DisciplinaId));

        if (repositorioMateria.SelecionarRegistros().Any(m => m.Nome == cadastrarVM.Nome))
        {
            ModelState.AddModelError("CadastroUnico", "Nome já cadastrado!");
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        Materia materia = new Materia(cadastrarVM.Nome, disciplina, cadastrarVM.Serie);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioMateria.CadastrarRegistro(materia);
            contexto.SaveChanges();
            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }

        return RedirectToAction("index");
    }


}
