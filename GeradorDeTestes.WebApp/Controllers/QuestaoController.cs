using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Aplicacao.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("questoes")]
public class QuestaoController : Controller
{
    private readonly QuestaoAppService questaoAppService;
    private readonly MateriaAppService materiaAppService;

    public QuestaoController(QuestaoAppService questaoAppService, MateriaAppService materiaAppService)
    {
        this.questaoAppService = questaoAppService;
        this.materiaAppService = materiaAppService;
    }

    public IActionResult Index()
    {
        Result<List<Questao>> resultadosQuestoesNaoFinalizadas = questaoAppService.SelecionarNaoFinalizados();

        if (resultadosQuestoesNaoFinalizadas.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Questao> questoesNaoFinalizadas = resultadosQuestoesNaoFinalizadas.Value;

        questaoAppService.RemoverRegistros(questoesNaoFinalizadas);

        Result<List<Questao>> resultadosQuestoes = questaoAppService.SelecionarRegistros();

        if (resultadosQuestoes.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Questao> questoes = resultadosQuestoes.Value.Where(q => q.Finalizado).ToList();

        VisualizarQuestoesViewModel visualizarVM = new(questoes);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        Result<List<Materia>> resultadosMaterias = materiaAppService.SelecionarRegistros();

        if (resultadosMaterias.IsFailed)
            return RedirectToAction(nameof(Index));

        List<Materia> materias = resultadosMaterias.Value;

        CadastrarQuestaoViewModel cadastrarVM = new(materias);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarQuestaoViewModel cadastrarVM)
    {
        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(cadastrarVM.MateriaId)!;

        Materia materiaSelecionada = resultadoMateria.Value;

        Questao novaQuestao = cadastrarVM.ParaEntidade(materiaSelecionada);

        Result resultadoCadastro = questaoAppService.CadastrarRegistro(novaQuestao);

        if (resultadoCadastro.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultadoCadastro.Errors[0].Message);

            return View(nameof(Cadastrar), cadastrarVM);
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id = novaQuestao.Id });
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

        if (resultadoQuestao.IsFailed)
            return RedirectToAction(nameof(Index));

        Questao questaoSelecionada = resultadoQuestao.Value;

        Result<List<Materia>> resultadosMaterias = materiaAppService.SelecionarRegistros();

        if (resultadosMaterias.IsFailed)
            return RedirectToAction(nameof(Index));

        List<Materia> materias = resultadosMaterias.Value;

        EditarQuestaoViewModel editarVM = new(
            questaoSelecionada,
            materias);

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    public IActionResult Editar(Guid id, EditarQuestaoViewModel editarVM)
    {
        Result<List<Questao>> resultadosQuestoes = questaoAppService.SelecionarRegistros();

        List<Questao> questoes = resultadosQuestoes.Value;

        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(editarVM.MateriaId)!;

        Materia materiaSelecionada = resultadoMateria.Value;

        Questao questaoEditada = editarVM.ParaEntidade(materiaSelecionada);

        Result resultadoEdicao = questaoAppService.EditarRegistro(id, questaoEditada);

        if (resultadoEdicao.IsFailed)
        {
            ModelState.AddModelError("ConflitoEdicao", resultadoEdicao.Errors[0].Message);

            return View(editarVM);
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

        if (resultadoQuestao.IsFailed)
            return RedirectToAction(nameof(Index));

        Questao questaoSelecionada = resultadoQuestao.Value;

        ExcluirQuestaoViewModel excluirVM = new(
            id,
            questaoSelecionada.Enunciado);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Result resultadoExclusao = questaoAppService.ExcluirRegistro(id);

        if (resultadoExclusao.IsFailed)
        {
            ModelState.AddModelError("ConflitoExclusao", resultadoExclusao.Errors[0].Message);

            Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

            Questao questaoSelecionada = resultadoQuestao.Value;

            ExcluirQuestaoViewModel excluirVM = new(
                id,
                questaoSelecionada.Enunciado);

            return View(nameof(Excluir), excluirVM);
        }

        return RedirectToAction("Index");
    }

    [HttpGet, Route("/questoes/{id:guid}/detalhes")]
    public IActionResult Detalhes(Guid id)
    {
        Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

        Questao questaoSelecionada = resultadoQuestao.Value;

        DetalhesQuestaoViewModel detalhesQuestaoVM = questaoSelecionada.ParaDetalhesVM();

        return View(detalhesQuestaoVM);
    }

    [HttpGet, Route("/questoes/{id:guid}/alternativas")]
    public IActionResult GerenciarAlternativas(Guid id)
    {
        Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

        Questao questaoSelecionada = resultadoQuestao.Value;

        List<Alternativa> alternativas = questaoSelecionada.Alternativas.ToList();

        GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
            questaoSelecionada,
            alternativas);

        if (alternativas.Count < 2 || alternativas.Count > 4)
            ModelState.AddModelError("ConflitoAlternativas", "A questão deve ter entre 2 e 4 alternativas.");

        if (alternativas.Count(a => a.EstaCorreta) != 1)
            ModelState.AddModelError("ConflitoAlternativas", "A questão deve ter exatamente uma alternativa correta.");

        if (!ModelState.IsValid)
            return View(gerenciarAlternativasVM);

        return View(gerenciarAlternativasVM);
    }

    [HttpPost, Route("/questoes/{id:guid}/adicionar-alternativa")]
    public IActionResult AdicionarAlternativa(Guid id, AdicionarAlternativaViewModel adicionarAlternativaVM)
    {
        Result resultadoAdicaoAlternativa = questaoAppService.AdicionarAlternativa(
            id, adicionarAlternativaVM.TextoAlternativa);

        if (resultadoAdicaoAlternativa.IsFailed)
        {
            ModelState.AddModelError("ConflitoAlternativas", resultadoAdicaoAlternativa.Errors[0].Message);

            Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

            Questao questaoSelecionada = resultadoQuestao.Value;

            GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
                questaoSelecionada,
                questaoSelecionada.Alternativas.ToList());

            return View(nameof(GerenciarAlternativas), gerenciarAlternativasVM);
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost, Route("/questoes/{id:guid}/remover-alternativa/{idAlternativa:guid}")]
    public IActionResult RemoverAlternativa(Guid id, Guid idAlternativa)
    {
        Result resultadoRemocaoAlternativa = questaoAppService.RemoverAlternativa(
            id, idAlternativa);

        if (resultadoRemocaoAlternativa.IsFailed)
        {
            ModelState.AddModelError("ConflitoAlternativas", resultadoRemocaoAlternativa.Errors[0].Message);

            Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

            Questao questaoSelecionada = resultadoQuestao.Value;

            GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
                questaoSelecionada,
                questaoSelecionada.Alternativas.ToList());

            return View(nameof(GerenciarAlternativas), gerenciarAlternativasVM);
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost("/questoes/{id:guid}/marcar-alternativa-correta")]
    public IActionResult MarcarAlternativaCorreta(Guid id, Guid idAlternativaCorreta)
    {
        Result resultadoMarcacao = questaoAppService.MarcarAlternativaCorreta(id, idAlternativaCorreta);

        if (resultadoMarcacao.IsFailed)
        {
            ModelState.AddModelError("ConflitoAlternativas", resultadoMarcacao.Errors[0].Message);

            Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

            Questao questaoSelecionada = resultadoQuestao.Value;

            GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
                questaoSelecionada,
                questaoSelecionada.Alternativas.ToList());

            return View(nameof(GerenciarAlternativas), gerenciarAlternativasVM);
        }

        return RedirectToAction(nameof(GerenciarAlternativas), new { id });
    }

    [HttpPost, Route("/questoes/{id:guid}/finalizar")]
    public IActionResult FinalizarQuestao(Guid id)
    {
        Result resultadoFinalizacao = questaoAppService.FinalizarQuestao(id);

        if (resultadoFinalizacao.IsFailed)
        {
            ModelState.AddModelError("ConflitoAlternativas", resultadoFinalizacao.Errors[0].Message);

            Result<Questao> resultadoQuestao = questaoAppService.SelecionarRegistroPorId(id)!;

            Questao questaoSelecionada = resultadoQuestao.Value;

            GerenciarAlternativasViewModel gerenciarAlternativasVM = new(
                questaoSelecionada,
                questaoSelecionada.Alternativas.ToList());

            return View(nameof(GerenciarAlternativas), gerenciarAlternativasVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("gerar-questoes/primeira-etapa")]
    public IActionResult PrimeiraEtapaGerar()
    {
        List<Materia> materias = materiaAppService.SelecionarRegistros().ValueOrDefault;

        PrimeiraEtapaGerarQuestoesViewModel primeiraEtapaVm = new PrimeiraEtapaGerarQuestoesViewModel(materias);

        return View(primeiraEtapaVm);
    }

    [HttpPost("gerar-questoes/primeira-etapa")]
    public async Task<IActionResult> PrimeiraEtapaGerar(PrimeiraEtapaGerarQuestoesViewModel primeiraEtapaVM)
    {
        Materia materiaSelecionada = materiaAppService.SelecionarRegistroPorId(primeiraEtapaVM.MateriaId).ValueOrDefault;

        Result<List<Questao>> resultado = await questaoAppService.GerarQuestoesDaMateria(materiaSelecionada, primeiraEtapaVM.QuantidadeQuestoes);

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

        string serieFormatada = (int)materiaSelecionada.Serie >= 10
        ? materiaSelecionada.Serie.GetDisplayName()[..8] : materiaSelecionada.Serie.GetDisplayName()[..7];

        SegundaEtapaGerarQuestoesViewModel segundaEtapaVM = new SegundaEtapaGerarQuestoesViewModel(resultado.Value)
        {
            MateriaId = primeiraEtapaVM.MateriaId,
            Materia = $"{materiaSelecionada.Nome} - {serieFormatada}"
        };

        string jsonString = JsonSerializer.Serialize(segundaEtapaVM);

        TempData.Clear();

        TempData.Add(nameof(SegundaEtapaGerarQuestoesViewModel), jsonString);

        return RedirectToAction(nameof(SegundaEtapaGerar));
    }

    [HttpGet("gerar-questoes/segunda-etapa")]
    public IActionResult SegundaEtapaGerar()
    {
        bool existeViewModel = TempData.TryGetValue(nameof(SegundaEtapaGerarQuestoesViewModel), out object? valor);

        if (!existeViewModel || valor is not string jsonString)
            return RedirectToAction(nameof(PrimeiraEtapaGerar));

        SegundaEtapaGerarQuestoesViewModel? segundaEtapaVM = JsonSerializer.Deserialize<SegundaEtapaGerarQuestoesViewModel>(jsonString);

        return View(segundaEtapaVM);
    }

    [HttpPost("gerar-questoes/segunda-etapa")]
    public IActionResult SegundaEtapaGerar(SegundaEtapaGerarQuestoesViewModel segundaEtapaVM)
    {
        List<Materia> materias = materiaAppService.SelecionarRegistros().ValueOrDefault;

        Materia materiaSelecionada = materiaAppService.SelecionarRegistroPorId(segundaEtapaVM.MateriaId).ValueOrDefault;

        List<Questao> questoesGeradas = SegundaEtapaGerarQuestoesViewModel.ObterQuestoesGeradas(segundaEtapaVM, materiaSelecionada);

        Result resultadoCadastroQuestoes = questaoAppService.CadastrarQuestoesGeradas(questoesGeradas);

        if (resultadoCadastroQuestoes.IsFailed)
            return View(nameof(SegundaEtapaGerar), segundaEtapaVM);

        return RedirectToAction(nameof(Index));
    }
}
