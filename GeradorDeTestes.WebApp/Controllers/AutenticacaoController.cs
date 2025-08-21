using FluentResults;
using GeradorDeTestes.Aplicacao.ModuloAutenticacao;
using GeradorDeTestes.Dominio.ModuloAutenticacao;
using GeradorDeTestes.WebApp.Extensions;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Controllers;

[Route("autenticacao")]
public class AutenticacaoController : Controller
{
    AutenticacaoAppService autenticacaoAppService;

    public AutenticacaoController(AutenticacaoAppService autenticacaoAppService)
    {
        this.autenticacaoAppService = autenticacaoAppService;
    }

    [HttpGet("registro")]
    public IActionResult Registro()
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        RegistroViewModel registroVm = new();

        return View(registroVm);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroViewModel registroVm)
    {
        Usuario novoUsuario = new()
        {
            UserName = registroVm.Email,
            Email = registroVm.Email
        };

        Result resultadoRegistro = await autenticacaoAppService.RegistrarAsync(
            novoUsuario,
            registroVm.Senha ?? string.Empty,
            registroVm.Tipo);

        if (resultadoRegistro.IsFailed)
            return RedirectToAction(nameof(Registro));


        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = string.Empty });
    }

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        LoginViewModel loginVm = new();

        ViewData["ReturnUrl"] = returnUrl;

        return View(loginVm);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginVm, string? returnUrl = null)
    {
        Result resultadoLogin = await autenticacaoAppService.LoginAsync(
            loginVm.Email ?? string.Empty,
            loginVm.Senha ?? string.Empty);

        if (resultadoLogin.IsFailed)
            return this.PreencherErrosModelState(resultadoLogin, loginVm);

        if (Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = string.Empty });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await autenticacaoAppService.LogoutAsync();

        return RedirectToAction(nameof(Login));
    }
}
