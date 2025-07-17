using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Controllers;

public class HomeController : Controller
{
    // private readonly IRepositorioTeste repositorioTeste;

    public HomeController(/*IRepositorioTeste repositorioTeste*/)
    {
        //this.repositorioTeste = repositorioTeste;
    }

    public IActionResult Index()
    {
        HomeViewModel homeVM = new();
        //{
        //    Teste = [.. repositorioTeste.SelecionarRegistros()
        //                        .OrderByDescending(t => t.DataCriacao)
        //                        .Take(5)
        //                        .Select(t => t.Titulo)]
        //};

        return View(homeVM);
    }

    [HttpGet("erro")]
    public IActionResult Erro()
    {
        return View();
    }
}
