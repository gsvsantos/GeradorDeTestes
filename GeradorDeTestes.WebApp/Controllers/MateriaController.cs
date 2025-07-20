using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Extensions;
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

        Materia materia = cadastrarVM.ParaEntidade(disciplina!);

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

    [HttpGet("editar/{id:Guid}")]
    public IActionResult Editar(Guid id)
    {
        Materia materia = repositorioMateria.SelecionarRegistroPorId(id)!;

        List<Disciplina> disciplinas = contexto.Disciplinas.ToList();

        if (materia == null)
            return NotFound();

        EditarMateriaViewModel editarVM = new EditarMateriaViewModel(materia, disciplinas);

        return View(editarVM);
    }

    [HttpPost("editar/{id:Guid}")]
    public IActionResult Editar(Guid id, EditarMateriaViewModel editarVM)
    {
        Disciplina? disciplina = contexto.Disciplinas
       .Include(disc => disc.Materias)
       .Include(disc => disc.Testes)
       .FirstOrDefault(disc => disc.Id.Equals(editarVM.DisciplinaId));

        Materia materiaEditada = editarVM.ParaEntidade(disciplina!);

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioMateria.EditarRegistro(id, materiaEditada);
            contexto.SaveChanges();
            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
        return RedirectToAction("Index");

    }

    [HttpGet("excluir/{id:Guid}")]
    public IActionResult Excluir(Guid id)
    {
        Materia materia = repositorioMateria.SelecionarRegistroPorId(id)!;

        if (materia == null)
            return NotFound();

        ExcluirMateriaViewModel excluirVM = new ExcluirMateriaViewModel(id, materia.Nome);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:Guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorioMateria.ExcluirRegistro(id);
            contexto.SaveChanges();
            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
        return RedirectToAction("Index");
    }
}
