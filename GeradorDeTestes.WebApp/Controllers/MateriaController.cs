using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("materias")]
public class MateriaController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioDisciplina repositorioDisciplina;
    public MateriaController(GeradorDeTestesDbContext contexto, IRepositorioMateria repoitorioMateria, IRepositorioDisciplina repositorioDisciplina)
    {
        this.contexto = contexto;
        this.repositorioMateria = repoitorioMateria;
        this.repositorioDisciplina = repositorioDisciplina;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        List<Materia> registros = repositorioMateria.SelecionarRegistros();
        VisualizarMateriaViewModel visualizarVM = new VisualizarMateriaViewModel(registros);
        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        List<Disciplina> disciplinas = repositorioDisciplina.SelecionarRegistros();

        CadastrarMateriaViewModel cadastrarVM = new CadastrarMateriaViewModel(disciplinas);
        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarMateriaViewModel cadastrarVM)
    {
        Disciplina disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(cadastrarVM.DisciplinaId)!;

        if (repositorioMateria.SelecionarRegistros().Any(m => m.Nome == cadastrarVM.Nome
        && m.Disciplina.Id == cadastrarVM.DisciplinaId && m.Serie == cadastrarVM.Serie))
        {
            ModelState.AddModelError("ConflitoCadastro", "Já existe uma matéria com este nome para a mesma disciplina e série.");
        }

        if (!ModelState.IsValid)
        {
            cadastrarVM.Disciplinas = repositorioDisciplina.SelecionarRegistros()
                .Select(d => new SelectListItem { Text = d.Nome, Value = d.Id.ToString() }).ToList();

            return View(cadastrarVM);
        }

        Materia materia = cadastrarVM.ParaEntidade(disciplinaSelecionada!);

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
        if (repositorioMateria.SelecionarRegistros().Any(m => m.Nome == editarVM.Nome
        && m.Disciplina.Id == editarVM.DisciplinaId && m.Serie == editarVM.Serie && m.Id != id))
        {
            ModelState.AddModelError("ConflitoEdicao", "Já existe outra matéria com este nome para a mesma disciplina e série.");
        }

        if (!ModelState.IsValid)
        {
            editarVM.Disciplinas = repositorioDisciplina.SelecionarRegistros()
                .Select(d => new SelectListItem { Text = d.Nome, Value = d.Id.ToString() }).ToList();

            return View(editarVM);
        }

        Disciplina disciplinaSelecionada = repositorioDisciplina.SelecionarRegistroPorId(editarVM.DisciplinaId)!;

        Materia materiaEditada = editarVM.ParaEntidade(disciplinaSelecionada!);

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
        Materia materia = repositorioMateria.SelecionarRegistroPorId(id)!;

        if (contexto.Questoes.Any(q => q.Materia.Id == id))
        {
            ModelState.AddModelError("ConflitoExclusao", "Não é possível excluir a matéria pois ela possui questões associadas.");

            ExcluirMateriaViewModel excluirVM = new()
            {
                Id = materia.Id,
                Nome = materia.Nome
            };

            return View("Excluir", excluirVM);
        }

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

    [HttpGet("detalhes/{id:Guid}")]
    public IActionResult Detalhes(Guid id)
    {
        Materia materia = repositorioMateria.SelecionarRegistroPorId(id)!;

        if (materia == null)
            return NotFound();

        DetalhesMateriaViewModel detalhesVM = materia.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
