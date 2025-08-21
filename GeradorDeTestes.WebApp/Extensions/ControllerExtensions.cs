using FluentResults;
using GeradorDeTestes.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeTestes.WebApp.Extensions;

public static class ControllerExtensions
{
    public static IActionResult RedirecionarParaNotificacao(this Controller controller, Result resultado)
    {
        foreach (IError? error in resultado.Errors)
        {
            string notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                error.Message,
                error.Reasons[0].Message
            );

            controller.TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
        }

        return controller.RedirectToAction("Index");
    }

    public static IActionResult PreencherErrosModelState(this Controller controller, Result resultado, object viewModel)
    {
        foreach (IError? erro in resultado.Errors)
        {
            string chave = erro.Metadata.TryGetValue("TipoErro", out object? tipo) ?
                tipo.ToString() ?? "ErroInesperado" : "ErroInesperado";

            foreach (IError? reason in erro.Reasons)
                controller.ModelState.AddModelError(chave, reason.Message);
        }

        return controller.View(viewModel);
    }
}
