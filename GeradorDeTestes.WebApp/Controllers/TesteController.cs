using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Aplicacao.ModuloQuestao;
using GeradorDeTestes.Aplicacao.ModuloTeste;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("testes")]
public class TesteController : Controller
{
    private readonly DisciplinaAppService disciplinaAppService;
    private readonly MateriaAppService materiaAppService;
    private readonly QuestaoAppService questaoAppService;
    private readonly TesteAppService testeAppService;
    private readonly GeradorPdfService geradorPdfService;

    public TesteController(DisciplinaAppService disciplinaAppService, MateriaAppService materiaAppService,
        QuestaoAppService questaoAppService, TesteAppService testeAppService,
        GeradorPdfService geradorPdfService)
    {
        this.disciplinaAppService = disciplinaAppService;
        this.materiaAppService = materiaAppService;
        this.questaoAppService = questaoAppService;
        this.testeAppService = testeAppService;
        this.geradorPdfService = geradorPdfService;
    }

    public IActionResult Index()
    {
        Result<List<Teste>> resultadosTestesNaoFinalizados = testeAppService.SelecionarNaoFinalizados();

        if (resultadosTestesNaoFinalizados.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Teste> testesNaoFinalizados = resultadosTestesNaoFinalizados.Value;

        testeAppService.RemoverRegistros(testesNaoFinalizados);

        Result<List<Teste>> resultadosTestes = testeAppService.SelecionarRegistros();

        if (resultadosTestes.IsFailed)
            return RedirectToAction("Index", "Home");

        List<Teste> testes = resultadosTestes.Value.Where(t => t.Finalizado).ToList();

        VisualizarTestesViewModel visualizarVM = new(testes);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

        List<Disciplina> disciplinas = resultadosDisciplinas.Value;

        CadastrarTesteViewModel cadastrarVM = new(disciplinas);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarTesteViewModel cadastrarVM)
    {
        Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

        List<Disciplina> disciplinas = resultadosDisciplinas.Value;

        Disciplina disciplina = disciplinas.FirstOrDefault(d => d.Id.Equals(cadastrarVM.DisciplinaId))!;

        Teste novoTeste = cadastrarVM.ParaEntidade(disciplina);

        Result resultadoCadastro = testeAppService.CadastrarRegistro(novoTeste);

        if (resultadoCadastro.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultadoCadastro.Errors[0].Message);

            cadastrarVM.Disciplinas = disciplinas.Select(d => new SelectListItem
            {
                Text = d.Nome,
                Value = d.Id.ToString()
            }).ToList();

            return View(cadastrarVM);
        }

        string tipoGeracao = cadastrarVM.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id = novoTeste.Id });
    }

    [HttpGet("gerar-teste")]
    public IActionResult GerarTeste(Guid id)
    {
        Result resultadoGeracao = testeAppService.GerarQuestoesParaTeste(id);

        if (resultadoGeracao.IsFailed)
        {
            ModelState.AddModelError("ConflitoGeral", resultadoGeracao.Errors[0].Message);
            return RedirectToAction("Index", "Home");
        }

        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        if (resultadoTeste.IsFailed)
            return RedirectToAction("Index", "Home");

        Teste testeSelecionado = resultadoTeste.Value;

        Result<List<Materia>> resultadosMaterias = materiaAppService.SelecionarRegistros();

        List<Materia> materias = resultadosMaterias.Value
            .Where(m => m.Disciplina.Equals(testeSelecionado.Disciplina))
            .Where(m => m.Serie.Equals(testeSelecionado.Serie))
            .ToList();

        FormGerarPostViewModel gerarTestePostVM = testeSelecionado.ParaGerarPostVM(materias, testeSelecionado.Materias);

        return View(gerarTestePostVM);
    }

    [HttpPost("gerar-teste")]
    public IActionResult GerarTeste(Guid id, FormGerarPostViewModel gerarTestePostVM)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        if (resultadoTeste.IsFailed)
            return RedirectToAction(nameof(Index));

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        Result resultadoFinalizacao = testeAppService.FinalizarTeste(id);

        if (resultadoFinalizacao.IsFailed)
        {
            ModelState.AddModelError("ConflitoGeracao", resultadoFinalizacao.Errors[0].Message);

            List<Materia> materias = materiaAppService.SelecionarRegistros().Value
                .Where(m => m.Disciplina.Id.Equals(testeSelecionado.Disciplina.Id)
                && m.Serie.Equals(testeSelecionado.Serie)).ToList();

            FormGerarViewModel formGerarVM = testeSelecionado.ParaGerarPostVM(materias, testeSelecionado.Materias);

            return View(nameof(GerarTeste), formGerarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, Route("/testes/{id:guid}/selecionar-materia/{materiaId:guid}")]
    public IActionResult SelecionarMateria(Guid id, Guid materiaId)
    {
        Result resultadoAdesao = testeAppService.AderirMateriaAoTeste(id, materiaId);

        if (resultadoAdesao.IsFailed)
        {
            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

            if (resultadoTeste.IsFailed)
                return RedirectToAction(nameof(Index));

            Teste testeSelecionado = resultadoTeste.ValueOrDefault;

            TempData["Erros"] = resultadoAdesao.Errors.Select(e => e.Message).ToList();
            ModelState.AddModelError("ConflitoGeracao", resultadoAdesao.Errors[0].Message);

            List<Materia> materias = materiaAppService.SelecionarRegistros().Value
                .Where(m => m.Disciplina.Id.Equals(testeSelecionado.Disciplina.Id)
                && m.Serie.Equals(testeSelecionado.Serie)).ToList();

            FormGerarViewModel formGerarVM = testeSelecionado.ParaGerarPostVM(materias, testeSelecionado.Materias);

            return View(nameof(GerarTeste), formGerarVM);
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpPost, Route("/testes/{id:guid}/remover-materia/{materiaId:guid}")]
    public IActionResult RemoverMateria(Guid id, Guid materiaId)
    {
        Result resultadoRemocao = testeAppService.RemoverMateriaDoTeste(id, materiaId);

        if (resultadoRemocao.IsFailed)
        {
            TempData["Erros"] = resultadoRemocao.Errors.Select(e => e.Message).ToList();
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpGet, Route("/testes/{id:guid}/definir-quantidade-questoes/{materiaId:guid}")]
    public IActionResult DefinirQuantidadeQuestoes(Guid id, Guid materiaId)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(materiaId);

        Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(testeSelecionado.Disciplina.Id);

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

        List<Questao> questoes = materiaSelecionada.Questoes.ToList();

        DefinirQuantidadeQuestoesViewModel vm = new()
        {
            Id = testeSelecionado.Id,
            Titulo = testeSelecionado.Titulo,
            NomeDisciplina = disciplinaSelecionada.Nome,
            Serie = testeSelecionado.Serie,
            MateriaId = materiaId,
            Questoes = questoes.Select(q => new SelectListItem()
            {
                Text = q.Enunciado,
                Value = q.Id.ToString()
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost, Route("/testes/{id:guid}/definir-quantidade-questoes/{materiaId:guid}")]
    public IActionResult DefinirQuantidadeQuestoes(Guid id, Guid materiaId, DefinirQuantidadeQuestoesPostViewModel postVM)
    {
        Result resultado = testeAppService.DefinirQuantidadeQuestoesPorMateria(id, materiaId, postVM.QuantidadeQuestoesMateria);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError("ConflitoQuantidadeQuestoesMateria", resultado.Errors[0].Message);

            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

            Teste testeSelecionado = resultadoTeste.ValueOrDefault;

            Result<Materia> resultadoMateria = materiaAppService.SelecionarRegistroPorId(materiaId);

            Materia materiaSelecionada = resultadoMateria.ValueOrDefault;

            Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(testeSelecionado.Disciplina.Id);

            Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

            List<Questao> questoes = materiaSelecionada.Questoes.ToList();

            DefinirQuantidadeQuestoesViewModel vm = new()
            {
                Id = testeSelecionado.Id,
                Titulo = testeSelecionado.Titulo,
                NomeDisciplina = disciplinaSelecionada.Nome,
                Serie = testeSelecionado.Serie,
                MateriaId = materiaId,
                Questoes = questoes.Select(q => new SelectListItem()
                {
                    Text = q.Enunciado,
                    Value = q.Id.ToString()
                }).ToList()
            };

            return View(nameof(DefinirQuantidadeQuestoes), vm);
        }

        return RedirectToAction(nameof(GerarTeste), new { id });
    }

    [HttpPost, Route("/testes/{id:guid}/aleatorizar-questoes")]
    public IActionResult AleatorizarQuestoes(Guid id)
    {
        Result resultado = testeAppService.AtualizarQuestoes(id);

        if (resultado.IsFailed)
            ModelState.AddModelError("ConflitoGeracao", resultado.Errors[0].Message);

        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id)!;

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        string tipoGeracao = testeSelecionado.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id });
    }

    [HttpGet("gerar-provao")]
    public IActionResult GerarProvao(Guid id)
    {
        Result<Teste> resultado = testeAppService.GerarQuestoesParaProvao(id);

        if (resultado.IsFailed)
            return RedirectToAction(nameof(Index));

        Teste teste = resultado.Value;

        List<Materia> materias = teste.Materias;

        FormGerarPostViewModel vm = teste.ParaGerarPostVM(materias, materias);

        return View(vm);
    }

    [HttpPost("gerar-provao")]
    public IActionResult GerarProvao(Guid id, FormGerarPostViewModel formGerarPostVM)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id)!;

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        Result resultadoFinalizacao = testeAppService.FinalizarTeste(id);

        if (resultadoFinalizacao.IsFailed)
        {
            ModelState.AddModelError("ConflitoGeracao", resultadoFinalizacao.Errors[0].Message);

            List<Materia> materias = materiaAppService.SelecionarRegistros().Value
                .Where(m => m.Disciplina.Id.Equals(testeSelecionado.Disciplina.Id) &&
                            m.Serie.Equals(testeSelecionado.Serie)).ToList();

            FormGerarViewModel formGerarVM = testeSelecionado.ParaGerarPostVM(materias, testeSelecionado.Materias);

            return View(nameof(GerarProvao), formGerarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("duplicar/{id:guid}")]
    public IActionResult Duplicar(Guid id)
    {
        DuplicarViewModel duplicarVM = new();

        return View(duplicarVM);
    }

    [HttpPost("duplicar/{id:guid}")]
    public IActionResult Duplicar(Guid id, DuplicarViewModel duplicarVM)
    {
        Result<Teste> resultadoDuplicacao = testeAppService.DuplicarTeste(id, duplicarVM.Titulo);

        if (resultadoDuplicacao.IsFailed)
        {
            ModelState.AddModelError("ConflitoCadastro", resultadoDuplicacao.Errors[0].Message);
            return View(duplicarVM);
        }

        Teste novoTeste = resultadoDuplicacao.Value;

        string tipoGeracao = novoTeste.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id = novoTeste.Id });
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        if (resultadoTeste.IsFailed)
            return RedirectToAction(nameof(Index));

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        ExcluirTesteViewModel excluirVM = new(id, testeSelecionado.Titulo);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult ExcluirConfirmado(Guid id)
    {
        Result resultadoExclusao = testeAppService.ExcluirRegistro(id);

        if (resultadoExclusao.IsFailed)
        {
            ModelState.AddModelError("ConflitoExclusao", resultadoExclusao.Errors[0].Message);

            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id)!;

            Teste testeSelecionado = resultadoTeste.Value;

            ExcluirQuestaoViewModel excluirVM = new(
                id,
                testeSelecionado.Titulo);

            return View(nameof(Excluir), excluirVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/testes/{id:guid}/detalhes-teste")]
    public IActionResult DetalhesTeste(Guid id)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        DetalhesTesteViewModel detalhesTesteVM = testeSelecionado.ParaDetalhesTesteVM();

        return View(detalhesTesteVM);
    }

    [HttpGet, Route("/testes/{id:guid}/detalhes-provao")]
    public IActionResult DetalhesProvao(Guid id)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        DetalhesProvaoViewModel detalhesProvaoVM = testeSelecionado.ParaDetalhesProvaoVM();

        return View(detalhesProvaoVM);
    }

    [HttpGet, Route("/testes/{id:guid}/gerar-pdf")]
    public IActionResult GerarPdf(Guid id)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        byte[] pdfBytes = geradorPdfService.GerarPdfTeste(testeSelecionado);

        return File(pdfBytes, "application/pdf");
    }

    [HttpGet, Route("/testes/{id:guid}/gerar-gabarito")]
    public IActionResult GerarGabaritoPdf(Guid id)
    {
        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        byte[] pdfBytes = geradorPdfService.GerarPdfGabarito(testeSelecionado);

        return File(pdfBytes, "application/pdf");
    }
}
