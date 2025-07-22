using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("disciplinas")]
public class DisciplinaController : Controller
{
    private readonly GeradorDeTestesDbContext contexto;
    private readonly IRepositorioDisciplina repositorio;

    public DisciplinaController(GeradorDeTestesDbContext contexto, IRepositorioDisciplina repositorio)
    {
        this.contexto = contexto;
        this.repositorio = repositorio;
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Disciplina> disciplinas = repositorio.SelecionarRegistros();

        VisualizarDisciplinasViewModel visualizarVM = new()
        {
            Registros = disciplinas.Select(d => d.ParaDetalhesVM()).ToList()
        };

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        return View(new CadastrarDisciplinaViewModel());
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarDisciplinaViewModel cadastrarVM)
    {
        if (repositorio.SelecionarRegistros().Any(d => d.Nome == cadastrarVM.Nome))
            ModelState.AddModelError("ConflitoCadastro", "Já existe uma disciplina com este nome.");

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        Disciplina disciplina = cadastrarVM.ParaEntidade();

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorio.CadastrarRegistro(disciplina);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(Guid id)
    {
        Disciplina disciplina = repositorio.SelecionarRegistroPorId(id)!;

        EditarDisciplinaViewModel editarVM = new()
        {
            Id = disciplina.Id,
            Nome = disciplina.Nome
        };

        return View(editarVM);
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(Guid id, EditarDisciplinaViewModel editarVM)
    {
        if (repositorio.SelecionarRegistros().Any(d => d.Nome == editarVM.Nome && d.Id != id))
            ModelState.AddModelError("ConflitoEdicao", "Já existe uma disciplina com este nome.");

        if (!ModelState.IsValid)
            return View(editarVM);

        Disciplina existente = repositorio.SelecionarRegistros()
                                   .FirstOrDefault(d => d.Nome == editarVM.Nome && d.Id != id)!;

        Disciplina disciplinaEditada = editarVM.ParaEntidade();

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorio.EditarRegistro(id, disciplinaEditada);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id}")]
    public IActionResult Excluir(Guid id)
    {
        Disciplina disciplina = repositorio.SelecionarRegistroPorId(id)!;

        ExcluirDisciplinaViewModel excluirVM = new()
        {
            Id = disciplina.Id,
            Nome = disciplina.Nome
        };

        return View(excluirVM);
    }

    [HttpPost("excluir/{id}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Disciplina disciplina = repositorio.SelecionarRegistroPorId(id)!;

        if (contexto.Materias.Any(m => m.Disciplina.Id == id) || contexto.Testes.Any(t => t.Disciplina.Id == id))
        {
            ModelState.AddModelError("ConflitoExclusao", "Não é possível excluir: há matérias ou testes vinculados.");

            ExcluirDisciplinaViewModel excluirVM = new()
            {
                Id = disciplina.Id,
                Nome = disciplina.Nome
            };

            return View("Excluir", excluirVM);
        }

        IDbContextTransaction transacao = contexto.Database.BeginTransaction();

        try
        {
            repositorio.ExcluirRegistro(id);

            contexto.SaveChanges();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id}")]
    public IActionResult Detalhes(Guid id)
    {
        Disciplina disciplina = repositorio.SelecionarRegistroPorId(id)!;

        DetalhesDisciplinaViewModel detalhesVM = disciplina.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
