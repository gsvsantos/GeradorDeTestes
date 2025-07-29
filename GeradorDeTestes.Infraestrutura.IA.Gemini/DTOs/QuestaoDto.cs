namespace GeradorDeTestes.Infraestrutura.IA.Gemini.DTOs;

public class QuestaoDto
{
    public string Enunciado { get; set; }
    public List<AlternativaDto> Alternativas { get; set; }
}
