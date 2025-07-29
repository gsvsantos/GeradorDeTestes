namespace GeradorDeTestes.Infraestrutura.IA.Gemini.ModuloQuestao.DTOs;

public class QuestaoDto
{
    public string Enunciado { get; set; }
    public List<AlternativaDto> Alternativas { get; set; }
}
