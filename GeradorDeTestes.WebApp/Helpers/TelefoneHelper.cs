namespace GeradorDeTestes.WebApp.Helpers;

public static class TelefoneHelper
{
    public static string FormatarTelefone(string telefone)
    {
        string? digitos = new([.. telefone.Where(char.IsDigit)]);

        return digitos.Length switch
        {
            11 => $"({digitos[..2]}) {digitos[2..7]}-{digitos[7..]}",
            10 => $"({digitos[..2]}) {digitos[2..6]}-{digitos[6..]}",
            _ => telefone
        };
    }
}
