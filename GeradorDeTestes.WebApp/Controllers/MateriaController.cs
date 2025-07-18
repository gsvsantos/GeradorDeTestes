using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Microsoft.AspNetCore.Components.Route("materias")]
public class MateriaController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    private readonly IRepositorioMateria repositorioMateria;

    public MateriaController(GeradorDeTestesDbContext contexto, IRepositorioMateria repoitorioMateria)
    {
        this.contexto = contexto;
        this.repositorioMateria = repoitorioMateria;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var registros = repositorioMateria.SelecionarRegistros();
        var visualizarVM = new VisulizarMateriaViewModel(registros);
        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        CadastrarMateriaViewModel cadastrarVM = new CadastrarMateriaViewModel();
        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarMateriaViewModel cadastrarVM)
    {
        if (repositorioMateria.SelecionarRegistros().Any(m => m.Nome == cadastrarVM.Nome))
        {
            ModelState.AddModelError("CadastroUnico", "Nome já cadastrado!");
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        Materia materia = new Materia(cadastrarVM.Nome, cadastrarVM.Disciplina, cadastrarVM.Serie);

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
