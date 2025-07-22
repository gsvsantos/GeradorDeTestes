using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Controllers;

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
        return View();
    }
}
