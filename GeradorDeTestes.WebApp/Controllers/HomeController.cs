using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GeradorDeTestes.WebApp.Controllers;

[Authorize(Roles = "Cliente,Empresa")]
public class HomeController : Controller
{
    private readonly IRepositorioTeste repositorioTeste;

    public HomeController(IRepositorioTeste repositorioTeste)
    {
        this.repositorioTeste = repositorioTeste;
    }

    public IActionResult Index()
    {
        List<Teste> testes = repositorioTeste.SelecionarRegistros();

        HomeViewModel homeVM = new(testes);

        return View(homeVM);
    }

    [HttpGet("erro")]
    public IActionResult Erro()
    {
        bool existeNotificacao = TempData.TryGetValue(nameof(NotificacaoViewModel), out object? valor);

        if (existeNotificacao && valor is string jsonString)
        {
            NotificacaoViewModel? notificacaoVm = JsonSerializer.Deserialize<NotificacaoViewModel>(jsonString);

            ViewData.Add(nameof(NotificacaoViewModel), notificacaoVm);
        }

        return View();
    }
}
