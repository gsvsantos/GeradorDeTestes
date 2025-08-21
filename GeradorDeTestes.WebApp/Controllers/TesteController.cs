using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloDisciplina;
using GeradorDeTestes.Aplicacao.ModuloMateria;
using GeradorDeTestes.Aplicacao.ModuloTeste;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("testes")]
[Authorize(Roles = "Cliente,Empresa")]
public class TesteController : Controller
{
    private readonly DisciplinaAppService disciplinaAppService;
    private readonly MateriaAppService materiaAppService;
    private readonly TesteAppService testeAppService;
    private readonly GeradorPdfService geradorPdfService;

    public TesteController(DisciplinaAppService disciplinaAppService, MateriaAppService materiaAppService,
        TesteAppService testeAppService, GeradorPdfService geradorPdfService)
    {
        this.disciplinaAppService = disciplinaAppService;
        this.materiaAppService = materiaAppService;
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

        CadastrarTesteViewModel cadastrarVM = new(disciplinas);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar(CadastrarTesteViewModel cadastrarVM)
    {
        Result<Disciplina> resultadoDisciplina = disciplinaAppService.SelecionarRegistroPorId(cadastrarVM.DisciplinaId)!;

        Disciplina disciplinaSelecionada = resultadoDisciplina.ValueOrDefault;

        Teste novoTeste = cadastrarVM.ParaEntidade(disciplinaSelecionada);

        Result resultadoCadastro = testeAppService.CadastrarRegistro(novoTeste);

        if (resultadoCadastro.IsFailed)
        {
            foreach (IError erro in resultadoCadastro.Errors)
            {
                if (!string.IsNullOrWhiteSpace(erro.Metadata["TipoErro"].ToString()))
                {
                    ModelState.AddModelError("ConflitoCadastro", erro.Reasons[0].Message);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

            Result<List<Disciplina>> resultadosDisciplinas = disciplinaAppService.SelecionarRegistros();

            cadastrarVM.Disciplinas = resultadosDisciplinas.Value
                .Select(d => new SelectListItem(d.Nome, d.Id.ToString()))
                .ToList();

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
            foreach (IError erro in resultadoGeracao.Errors)
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

        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

        if (resultadoTeste.IsFailed)
        {
            foreach (IError erro in resultadoTeste.Errors)
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
        Result resultadoFinalizacao = testeAppService.FinalizarTeste(id);

        if (resultadoFinalizacao.IsFailed)
        {
            foreach (IError erro in resultadoFinalizacao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else if (erro.Metadata["TipoErro"].ToString() == "QuantidadeQuestoes")
                {
                    ModelState.AddModelError("ConflitoGeracao", erro.Reasons[0].Message);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

            Teste testeSelecionado = resultadoTeste.ValueOrDefault;

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
            foreach (IError erro in resultadoAdesao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ConflitoGeracao", erro.Reasons[0].Message);
                    break;
                }
            }

            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

            Teste testeSelecionado = resultadoTeste.ValueOrDefault;

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
            foreach (IError erro in resultadoRemocao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ConflitoGeracao", erro.Reasons[0].Message);
                    break;
                }
            }

            Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id);

            Teste testeSelecionado = resultadoTeste.ValueOrDefault;

            List<Materia> materias = materiaAppService.SelecionarRegistros().Value
                .Where(m => m.Disciplina.Id.Equals(testeSelecionado.Disciplina.Id)
                && m.Serie.Equals(testeSelecionado.Serie)).ToList();

            FormGerarViewModel formGerarVM = testeSelecionado.ParaGerarPostVM(materias, testeSelecionado.Materias);

            return View(nameof(GerarTeste), formGerarVM);
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
        Result resultadoDefinicaoQuantidade = testeAppService.DefinirQuantidadeQuestoesPorMateria(id, materiaId, postVM.QuantidadeQuestoesMateria);

        if (resultadoDefinicaoQuantidade.IsFailed)
        {
            foreach (IError erro in resultadoDefinicaoQuantidade.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ConflitoQuantidadeQuestoesMateria", erro.Reasons[0].Message);
                    break;
                }
            }

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
        Result resultadoAtualizacao = testeAppService.AtualizarQuestoes(id);

        if (resultadoAtualizacao.IsFailed)
        {
            foreach (IError erro in resultadoAtualizacao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ConflitoGeracao", erro.Reasons[0].Message);
                    break;
                }
            }
        }

        Result<Teste> resultadoTeste = testeAppService.SelecionarRegistroPorId(id)!;

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        string tipoGeracao = testeSelecionado.EhProvao ? nameof(GerarProvao) : nameof(GerarTeste);

        return RedirectToAction(tipoGeracao, new { id });
    }

    [HttpGet("gerar-provao")]
    public IActionResult GerarProvao(Guid id)
    {
        Result<Teste> resultadoGeracao = testeAppService.GerarQuestoesParaProvao(id);

        if (resultadoGeracao.IsFailed)
        {
            foreach (IError erro in resultadoGeracao.Errors)
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

        Teste teste = resultadoGeracao.Value;

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
            foreach (IError erro in resultadoFinalizacao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroNaoEncontrado")
                {

                    string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                            erro.Message,
                            erro.Reasons[0].Message
                        );

                    TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                    return RedirectToAction(nameof(Index));
                }
                else if (erro.Metadata["TipoErro"].ToString() == "QuantidadeQuestoes")
                {
                    ModelState.AddModelError("ConflitoGeracao", erro.Reasons[0].Message);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

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
            foreach (IError erro in resultadoDuplicacao.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroDuplicado")
                {
                    ModelState.AddModelError("ConflitoCadastro", erro.Reasons[0].Message);
                    break;
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }

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
        {
            foreach (IError erro in resultadoTeste.Errors)
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
            foreach (IError erro in resultadoExclusao.Errors)
            {
                ModelState.AddModelError("ConflitoExclusao", resultadoExclusao.Errors[0].Message);
                break;
            }

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

        if (resultadoTeste.IsFailed)
        {

            foreach (IError erro in resultadoTeste.Errors)
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

        Teste testeSelecionado = resultadoTeste.ValueOrDefault;

        DetalhesTesteViewModel detalhesTesteVM = testeSelecionado.ParaDetalhesTesteVM();

        return View(detalhesTesteVM);
    }

    [HttpGet, Route("/testes/{id:guid}/detalhes-provao")]
    public IActionResult DetalhesProvao(Guid id)
    {
        Result<Teste> resultadoProvao = testeAppService.SelecionarRegistroPorId(id);

        if (resultadoProvao.IsFailed)
        {

            foreach (IError erro in resultadoProvao.Errors)
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

        Teste provaoSelecionado = resultadoProvao.ValueOrDefault;

        DetalhesProvaoViewModel detalhesProvaoVM = provaoSelecionado.ParaDetalhesProvaoVM();

        return View(detalhesProvaoVM);
    }

    [HttpGet, Route("/testes/{id:guid}/gerar-pdf")]
    public IActionResult GerarPdf(Guid id)
    {
        Result<Teste> resultado = testeAppService.SelecionarRegistroPorId(id);

        if (resultado.IsFailed)
        {
            foreach (IError erro in resultado.Errors)
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

        Teste registroSelecionado = resultado.ValueOrDefault;

        byte[] pdfBytes = geradorPdfService.GerarPdfTeste(registroSelecionado);

        return File(pdfBytes, "application/pdf");
    }

    [HttpGet, Route("/testes/{id:guid}/gerar-gabarito")]
    public IActionResult GerarGabaritoPdf(Guid id)
    {
        Result<Teste> resultado = testeAppService.SelecionarRegistroPorId(id);

        if (resultado.IsFailed)
        {
            foreach (IError erro in resultado.Errors)
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

        Teste registroSelecionado = resultado.ValueOrDefault;

        byte[] pdfBytes = geradorPdfService.GerarPdfGabarito(registroSelecionado);

        return File(pdfBytes, "application/pdf");
    }
}
