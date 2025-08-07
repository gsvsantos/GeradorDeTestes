using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

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

        bool existeNotificacao = TempData.TryGetValue(nameof(NotificacaoViewModel), out object? valor);

        if (existeNotificacao && valor is string jsonString)
        {
            NotificacaoViewModel? notificacaoVM = JsonSerializer.Deserialize<NotificacaoViewModel>(jsonString);

            ViewData.Add(nameof(NotificacaoViewModel), notificacaoVM);
        }

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

        if (resultadosDisciplinas.IsFailed)
        {

            foreach (IError erro in resultadosDisciplinas.Errors)
            {
                string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }

            return RedirectToAction(nameof(Index));
        }

        List<Disciplina> disciplinas = resultadosDisciplinas.Value;

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
            foreach (IError erro in resultadoCadastro.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroDuplicado")
                {
                    ModelState.AddModelError("ConflitoCadastro", erro.Reasons[0].Message);
                    break;
                }
            }

            Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

            cadastrarVM.Disciplinas = resultadosDisciplinas.Value
                .Select(d => new SelectListItem(d.Nome, d.Id.ToString()))
                .ToList();

            return View(nameof(Cadastrar), cadastrarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:Guid}")]
    public IActionResult Editar(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
        {
            foreach (IError erro in resultadoMateria.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

        if (resultadosDisciplinas.IsFailed)
        {

            foreach (IError erro in resultadosDisciplinas.Errors)
            {
                string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }

            return RedirectToAction(nameof(Index));
        }

        List<Disciplina> disciplinas = resultadosDisciplinas.Value;

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
            foreach (IError erro in resultadoEdicao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroDuplicado")
                {
                    ModelState.AddModelError("ConflitoEdicao", resultadoEdicao.Errors[0].Message);
                    break;
                }
            }

            return View(nameof(Editar), editarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:Guid}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
        {

            foreach (IError erro in resultadoMateria.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

            return RedirectToAction(nameof(Index));
        }

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
            foreach (IError erro in resultadoExclusao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroVinculado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:Guid}")]
    public IActionResult Detalhes(Guid id)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(id)!;

        if (resultadoMateria.IsFailed)
        {

            foreach (IError erro in resultadoMateria.Errors)
            {
                string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }

            return RedirectToAction(nameof(Index));
        }

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        DetalhesMateriaViewModel detalhesVM = materiaSelecionada.ParaDetalhesVM();

        return View(detalhesVM);
    }
}
