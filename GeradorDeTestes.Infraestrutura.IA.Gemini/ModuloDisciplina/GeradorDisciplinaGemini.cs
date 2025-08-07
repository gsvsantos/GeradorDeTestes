using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Infraestrutura.IA.Gemini.ModuloDisciplina.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GeradorDeTestes.Infraestrutura.IA.Gemini.ModuloDisciplina;
public class GeradorDisciplinaGemini : IGeradorDisciplinas
{
    private readonly HttpClient _httpClient;
    private readonly string _geminiEndpoint;
    private static readonly Dictionary<string, string> Equivalencias = new()
    {
        { "arte", "arte e cultura" },
        { "ciências", "ciências da natureza" },
        { "ciência", "ciências da natureza" },
        { "educação financeira", "finanças" },
        { "inglês", "língua inglesa" },
        { "filosofia", "filosofia" },
        { "história", "história" },
        { "sociologia", "sociologia" }
    };
    private readonly int maxTentativas;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public GeradorDisciplinaGemini(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        string? geminiKey = configuration["GEMINI_API_KEY"];

        if (string.IsNullOrWhiteSpace(geminiKey))
            throw new Exception("A variável GEMINI_API_KEY não foi fornecida.");

        _httpClient = httpClientFactory.CreateClient();

        _geminiEndpoint = string.Concat("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=",
            geminiKey);
        maxTentativas = int.Parse(configuration["GeradorDisciplinas:MaxTentativasIA"] ?? "5");
    }

    public async Task<List<Disciplina>> GerarDisciplinasAsync(int quantidadeDesejada, List<Disciplina> disciplinasExistentes)
    {
        HashSet<string> nomesExistentes = disciplinasExistentes
            .Select(d => NormalizarNome(d.Nome))
            .ToHashSet();

        List<Disciplina> disciplinasNovas = new();

        int tentativas = 0;

        while (disciplinasNovas.Count < quantidadeDesejada && tentativas < maxTentativas)
        {
            int faltamGerar = quantidadeDesejada - disciplinasNovas.Count;

            List<DisciplinaDto> geradas = await GerarComGeminiAsync(faltamGerar, nomesExistentes);

            foreach (DisciplinaDto dto in geradas)
            {
                if (disciplinasNovas.Count >= quantidadeDesejada)
                    break;

                string nomePadrao = NormalizarNome(dto.Nome);

                bool jaExiste =
                    nomesExistentes.Contains(nomePadrao) ||
                    disciplinasNovas.Any(d => NormalizarNome(d.Nome) == nomePadrao);

                if (jaExiste || string.IsNullOrWhiteSpace(dto.Nome))
                    continue;

                Disciplina disciplina = new(dto.Nome.Trim());
                disciplinasNovas.Add(disciplina);
            }

            tentativas++;
        }

        return disciplinasNovas;
    }

    private async Task<List<DisciplinaDto>> GerarComGeminiAsync(int quantidade, HashSet<string> nomesExistentes)
    {
        string nomesExistentesJson = string.Join(", ", nomesExistentes.Select(nome => $"\"{nome}\""));

        string prompt = @$"
        Gere {quantidade} disciplinas escolares válidas para uso em escolas brasileiras, com base na Base Nacional Comum Curricular (BNCC).

        As disciplinas devem considerar os níveis de ensino:
        - Ensino Fundamental (anos iniciais e finais)
        - Ensino Médio

        Critérios:
        - A lista deve conter **nomes únicos** e **padronizados** (ex: 'Matemática', 'Língua Portuguesa', 'Filosofia', 'Projeto de Vida').
        - Evite repetições ou variações desnecessárias.
        - Evite nomes similares aos já existentes (como variações com sinônimos, diminutivos ou duplicações com pequenas diferenças).
        - Não vincule a disciplina a nenhum ano/série específico.
        - Pode incluir disciplinas obrigatórias e opcionais (como 'Educação Financeira', 'Robótica', 'Empreendedorismo', etc).
        
        Considere também os seguintes nomes já cadastrados (evite gerar nomes iguais ou similares):

        [{nomesExistentesJson}]        

        Formato de resposta JSON:
        [
          {{ ""Nome"": ""string"" }},
          ...
        ]
        ";

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

        HttpResponseMessage response = await EnviarComRetryAsync(content);

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

        return JsonSerializer.Deserialize<List<DisciplinaDto>>(text, _jsonSerializerOptions) ?? [];
    }

    private async Task<HttpResponseMessage> EnviarComRetryAsync(StringContent content)
    {
        const int maxTentativas = 3;
        int tentativa = 0;

        while (tentativa < maxTentativas)
        {
            HttpResponseMessage response = await _httpClient.PostAsync(_geminiEndpoint, content);

            if (response.IsSuccessStatusCode)
                return response;

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                tentativa++;
                await Task.Delay(2000 * tentativa);
                continue;
            }

            response.EnsureSuccessStatusCode();
        }

        throw new HttpRequestException("Serviço indisponível após múltiplas tentativas (503).");
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

    private static string NormalizarNome(string nome)
    {
        string nomeNormalizado = nome.Trim().ToLowerInvariant();

        return Equivalencias.TryGetValue(nomeNormalizado, out string? padrao)
            ? padrao
            : nomeNormalizado;
    }
}