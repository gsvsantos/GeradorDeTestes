using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Infraestrutura.IA.Gemini.DTOs;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace GeradorDeTestes.Infraestrutura.IA.Gemini;

public class GeradorQuestoesGemini : IGeradorQuestoes
{
    private readonly HttpClient _httpClient;
    private readonly string _geminiEndpoint;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public GeradorQuestoesGemini(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();

        _geminiEndpoint = string.Concat("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=",
            configuration["GEMINI_API_KEY"]);
    }

    public async Task<List<Questao>> GerarQuestoesAsync(Materia materia, int quantidade)
    {
        string prompt = @$"
            Gere {quantidade} questões de múltipla escolha sobre o tema: '{materia.Disciplina.Nome}-{materia.Nome}'.
            Cada questão deve ter:
            - Um enunciado
            - 4 alternativas com letras A, B, C e D
            - Apenas uma alternativa correta

            Retorne em formato JSON com a estrutura:
            [
              {{
                ""Enunciado"": ""string"",
                ""Alternativas"": [
                  {{ ""Letra"": ""A"", ""Resposta"": ""string"", ""Correta"": true/false }},
                  ...
                ]
              }},
              ...
            ]"
        ;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        StringContent content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_geminiEndpoint, content);

        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(responseString);

        string? text = doc.RootElement
                      .GetProperty("candidates")[0]
                      .GetProperty("content")
                      .GetProperty("parts")[0]
                      .GetProperty("text")
                      .GetString();

        text = ProcessarTexto(text);

        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Não foi possível reconhecer o formato de resposta do modelo.");

        List<QuestaoDto> questoes = JsonSerializer.Deserialize<List<QuestaoDto>>(text, _jsonSerializerOptions) ?? [];

        List<Questao> resultado = new List<Questao>();

        foreach (QuestaoDto dto in questoes)
        {
            Questao questao = new Questao(dto.Enunciado, materia);

            foreach (AlternativaDto alt in dto.Alternativas)
            {
                questao.AderirAlternativa(new()
                {
                    Texto = alt.Resposta,
                    EstaCorreta = alt.Correta
                });
            }

            resultado.Add(questao);
        }

        return resultado;
    }

    private static string ProcessarTexto(string? text)
    {
        text = text?.Trim() ?? string.Empty;

        if (text.StartsWith("```json"))
        {
            text = text.Replace("```json", "").Trim();
        }
        if (text.EndsWith("```"))
        {
            text = text.Substring(0, text.LastIndexOf("```")).Trim();
        }

        return text;
    }
}