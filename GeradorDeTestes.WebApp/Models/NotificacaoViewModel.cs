using System.Text.Json;

namespace GeradorDeTestes.WebApp.Models;

public class NotificacaoViewModel
{
    public required string Titulo { get; set; }
    public required string Mensagem { get; set; }

    public static string GerarNotificacaoSerializada(string titulo, string mensagem)
    {
        NotificacaoViewModel notificacao = new()
        {
            Titulo = titulo,
            Mensagem = mensagem
        };

        return JsonSerializer.Serialize(notificacao);
    }
}
