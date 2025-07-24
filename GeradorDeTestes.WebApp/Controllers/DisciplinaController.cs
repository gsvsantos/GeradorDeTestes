using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("disciplinas")]
public class DisciplinaController : Controller
{
    private readonly DisciplinaAppService disciplinaAppService;

    public DisciplinaController(DisciplinaAppService disciplinaAppService)
    {
        this.disciplinaAppService = disciplinaAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        Result<List<Disciplina>> resultado = disciplinaAppService.SelecionarRegistros();

        if (resultado.IsFailed)
            return RedirectToAction("Home/Index");

        List<Disciplina> registros = resultado.Value;

        VisualizarDisciplinasViewModel visualizarVM = new()
        {
            Registros = registros.Select(d => d.ParaDetalhesVM()).ToList()
        };

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        CadastrarDisciplinaViewModel cadastrarVM = new();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarDisciplinaViewModel cadastrarVM)
    {
        Disciplina disciplina = cadastrarVM.ParaEntidade();

        Result resultado = disciplinaAppService.Cadastrar(disciplina);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultado.Errors[0].Message);

            return View(cadastrarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(Guid id)
    {
        Result<Disciplina> resultado = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (resultado.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplinaSelecionada = resultado.ValueOrDefault;

        EditarDisciplinaViewModel editarVM = new()
        {
            Id = disciplinaSelecionada.Id,
            Nome = disciplinaSelecionada.Nome
        };

        return View(editarVM);
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(Guid id, EditarDisciplinaViewModel editarVM)
    {
        Disciplina disciplinaEditada = editarVM.ParaEntidade();

        Result<string> resultado = disciplinaAppService.Editar(id, disciplinaEditada);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError("ConflitoEdicao", resultado.Errors[0].Message);

            return View(editarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Disciplina> resultado = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (resultado.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplinaSelecionada = resultado.ValueOrDefault;

        ExcluirDisciplinaViewModel excluirVM = new()
        {
            Id = disciplinaSelecionada.Id,
            Nome = disciplinaSelecionada.Nome
        };

        return View(excluirVM);
    }

    [HttpPost("excluir/{id}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Result resultado = disciplinaAppService.Excluir(id);

        if (resultado.IsFailed)
        {
            Result<Disciplina> disciplinaSelecionada = disciplinaAppService.SelecionarRegistroPorId(id)!;
            Disciplina disciplina = disciplinaSelecionada.ValueOrDefault;

            ModelState.AddModelError("ConflitoExclusao", resultado.Errors[0].Message);

            ExcluirDisciplinaViewModel excluirVM = new()
            {
                Id = disciplina.Id,
                Nome = disciplina.Nome
            };

            return View(nameof(Excluir), excluirVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id}")]
    public IActionResult Detalhes(Guid id)
    {
        Result<Disciplina> disciplinaSelecionada = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (disciplinaSelecionada.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplina = disciplinaSelecionada.ValueOrDefault;

        DetalhesDisciplinaViewModel detalhesVM = disciplina.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
