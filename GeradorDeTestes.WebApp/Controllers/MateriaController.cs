using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("materias")]
public class MateriaController : Controller
{
    private readonly DisciplinaAppService disciplinaAppService;
    private readonly MateriaAppService materiaAppService;

    public MateriaController(DisciplinaAppService disciplinaAppService, MateriaAppService materiaAppService)
    {
        this.disciplinaAppService = disciplinaAppService;
        this.materiaAppService = materiaAppService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        Result<List<Materia>> resultadosMaterias = materiaAppService.SelecionarRegistros();

        if (resultadosMaterias.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Materia> materias = resultadosMaterias.Value;

        VisualizarMateriaViewModel visualizarVM = new(materias);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        Result<List<Disciplina>> resultadoDisciplinas = disciplinaAppService.SelecionarRegistros();

        List<Disciplina> disciplinas = resultadoDisciplinas.Value;

        CadastrarMateriaViewModel cadastrarVM = new(disciplinas);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarMateriaViewModel cadastrarVM)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(cadastrarVM.DisciplinaId)!;

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

        Materia novaMateria = cadastrarVM.ParaEntidade(disciplinaSelecionada!);

        Result resultadoCadastro = materiaAppService.CadastrarRegistro(novaMateria);

        if (resultadoCadastro.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultadoCadastro.Errors[0].Message);

            return View(nameof(Cadastrar), cadastrarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:Guid}")]
    public IActionResult Editar(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
            return RedirectToAction(nameof(Index));

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        Result<List<Disciplina>> resultadoDisciplinas = disciplinaAppService.SelecionarRegistros();

        List<Disciplina> disciplinas = resultadoDisciplinas.Value;

        EditarMateriaViewModel editarVM = new(materiaSelecionada, disciplinas);

        return View(nameof(Editar), editarVM);
    }

    [HttpPost("editar/{id:Guid}")]
    public IActionResult Editar(Guid id, EditarMateriaViewModel editarVM)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(editarVM.DisciplinaId)!;

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

        Materia materiaEditada = editarVM.ParaEntidade(disciplinaSelecionada!);

        Result resultadoEdicao = materiaAppService.EditarRegistro(id, materiaEditada);

        if (resultadoEdicao.IsFailed)
        {
            ModelState.AddModelError("ConflitoEdicao", resultadoEdicao.Errors[0].Message);

            return View(editarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:Guid}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
            return RedirectToAction(nameof(Index));

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        ExcluirMateriaViewModel excluirVM = new(id, materiaSelecionada.Nome);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:Guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Result resultadoExclusao = materiaAppService.ExcluirRegistro(id)!;

        if (resultadoExclusao.IsFailed)
        {
            ModelState.AddModelError("ConflitoExclusao", "Não é possível excluir a matéria pois ela possui questões associadas.");

            Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

            Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

            ExcluirMateriaViewModel excluirVM = new()
            {
                Id = materiaSelecionada.Id,
                Nome = materiaSelecionada.Nome
            };

            return View(nameof(Excluir), excluirVM);
        }

        return RedirectToAction("Index");
    }

    [HttpGet("detalhes/{id:Guid}")]
    public IActionResult Detalhes(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
            return RedirectToAction(nameof(Index));

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        DetalhesMateriaViewModel detalhesVM = materiaSelecionada.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
