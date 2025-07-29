using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

    [HttpGet("gerar-disciplinas/primeira-etapa")]
    public IActionResult PrimeiraEtapaGerar()
    {
        PrimeiraEtapaGerarDisciplinasViewModel primeiraEtapaVM = new();

        return View(primeiraEtapaVM);
    }

    [HttpPost("gerar-disciplinas/primeira-etapa")]
    public async Task<IActionResult> PrimeiraEtapaGerar(PrimeiraEtapaGerarDisciplinasViewModel primeiraEtapaVM)
    {
        Result<List<Disciplina>> resultado = await disciplinaAppService.GerarDisciplinas(primeiraEtapaVM.QuantidadeDisciplinas);

        if (resultado.Value.Count == 0)
        {
            TempData["ConflitoGeracao"] = "Nenhuma nova disciplina foi gerada. Tente novamente.";
            return RedirectToAction(nameof(PrimeiraEtapaGerar));
        }

        if (resultado.IsFailed)
        {
            foreach (IError? erro in resultado.Errors)
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

        SegundaEtapaGerarDisciplinasViewModel segundaEtapavm = new(resultado.Value);

        string jsonString = JsonSerializer.Serialize(segundaEtapavm);

        TempData.Clear();

        TempData.Add(nameof(SegundaEtapaGerarDisciplinasViewModel), jsonString);

        return RedirectToAction(nameof(SegundaEtapaGerar));
    }

    [HttpGet("gerar-disciplinas/segunda-etapa")]
    public IActionResult SegundaEtapaGerar()
    {
        bool existeViewModel = TempData.TryGetValue(nameof(SegundaEtapaGerarDisciplinasViewModel), out object? valor);

        if (!existeViewModel || valor is not string jsonString)
            return RedirectToAction(nameof(PrimeiraEtapaGerar));

        SegundaEtapaGerarDisciplinasViewModel? segundaEtapaVm = JsonSerializer.Deserialize<SegundaEtapaGerarDisciplinasViewModel>(jsonString);

        return View(segundaEtapaVm);
    }

    [HttpPost("gerar-disciplinas/segunda-etapa")]
    public IActionResult SegundaEtapaGerar(SegundaEtapaGerarDisciplinasViewModel segundaEtapaVm)
    {
        List<Disciplina> disciplinasGeradas = SegundaEtapaGerarDisciplinasViewModel.ObterDisciplinasGeradas(segundaEtapaVm);

        foreach (Disciplina disciplina in disciplinasGeradas)
        {
            Result resultado = disciplinaAppService.CadastrarRegistro(disciplina);

            if (resultado.IsFailed)
                return View(nameof(PrimeiraEtapaGerar));
        }

        return RedirectToAction(nameof(Index));
    }
}
