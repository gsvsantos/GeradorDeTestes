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
        Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

        if (resultadosDisciplinas.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Disciplina> disciplinas = resultadosDisciplinas.Value;

        VisualizarDisciplinasViewModel visualizarVM = new()
        {
            Registros = disciplinas.Select(d => d.ParaDetalhesVM()).ToList()
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
        Disciplina novaDisciplina = cadastrarVM.ParaEntidade();

        Result resultadoCadastro = disciplinaAppService.CadastrarRegistro(novaDisciplina);

        if (resultadoCadastro.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultadoCadastro.Errors[0].Message);

            return View(nameof(Cadastrar), cadastrarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(Guid id)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoDisciplina.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

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

        Result resultadoEdicao = disciplinaAppService.EditarRegistro(id, disciplinaEditada);

        if (resultadoEdicao.IsFailed)
        {
            ModelState.AddModelError("ConflitoEdicao", resultadoEdicao.Errors[0].Message);

            return View(nameof(Editar), editarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoDisciplina.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

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
        Result resultadoExclusao = disciplinaAppService.ExcluirRegistro(id);

        if (resultadoExclusao.IsFailed)
        {
            Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(id)!;

            Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

            ModelState.AddModelError("ConflitoExclusao", resultadoExclusao.Errors[0].Message);

            ExcluirDisciplinaViewModel excluirVM = new()
            {
                Id = disciplinaSelecionada.Id,
                Nome = disciplinaSelecionada.Nome
            };

            return View(nameof(Excluir), excluirVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id}")]
    public IActionResult Detalhes(Guid id)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoDisciplina.IsFailed)
            return RedirectToAction(nameof(Index));

        Disciplina disciplinaSelecionda = resultadoDisciplina.ValueOrDefault;

        DetalhesDisciplinaViewModel detalhesVM = disciplinaSelecionda.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
