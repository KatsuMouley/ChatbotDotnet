using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "";  // Substitua pela sua chave da API

    public ChatController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // [HttpPost("ask")]
    // public async Task<IActionResult> AskQuestion([FromBody] UserRequest request)
    // {
    //     try
    //     {
    //         // Faz a chamada à API Gemini
    //         var geminiResponse = await SendMessageToGeminiAsync(request.Question);

    //         // Log temporário para exibir o conteúdo bruto da resposta da API
    //         Console.WriteLine("Resposta da API Gemini (bruta): " + geminiResponse);

    //         // Processa a resposta da API Gemini
    //         var responseObject = JsonSerializer.Deserialize<GeminiResponse>(geminiResponse);

    //         // Verifica se a resposta contém dados válidos
    //         if (responseObject?.Candidates != null && responseObject.Candidates.Any())
    //         {
    //             var answer = responseObject.Candidates.First().Content?.Parts?.FirstOrDefault()?.Text ?? "Nenhuma resposta foi gerada.";

    //             // Extrai palavras-chave
    //             var keywords = ExtractKeywords(answer);

    //             // Retorna a resposta e palavras-chave
    //             return Ok(new
    //             {
    //                 response = answer,
    //                 keywords = keywords
    //             });
    //         }
    //         else
    //         {
    //             // Log temporário para verificar o objeto deserializado
    //             Console.WriteLine("Objeto deserializado da API Gemini: " + JsonSerializer.Serialize(responseObject));

    //             return BadRequest(new { error = "Resposta inválida ou vazia da API Gemini." });
    //         }
    //     }
    //     catch (HttpRequestException ex)
    //     {
    //         return BadRequest(new { error = ex.Message });
    //     }
    // }

    [HttpPost("ask")]
public async Task<IActionResult> AskQuestion([FromBody] UserRequest request)
{
    try
    {
        // Faz a chamada à API Gemini e recebe a resposta bruta
        var geminiResponse = await SendMessageToGeminiAsync(request.Question);

        // Retorna a resposta da API Gemini diretamente
        return Ok(geminiResponse);
    }
    catch (HttpRequestException ex)
    {
        // Retorna um erro em caso de falha na chamada à API
        return BadRequest(new { error = ex.Message });
    }
}


    // Função para fazer a chamada à API Gemini
    private async Task<string> SendMessageToGeminiAsync(string userMessage)
    {
        var geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey;

        // Configuração do request para a API Gemini
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = "Responda a frase "+userMessage+" com no máximo 100 caracteres" }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 1,
                topK = 64,
                topP = 0.95,
                maxOutputTokens = 8192,
                responseMimeType = "text/plain"
            }
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Faz a requisição POST para a API Gemini
        var response = await _httpClient.PostAsync(geminiEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return responseBody;  // Retorna a resposta do Gemini
        }
        else
        {
            throw new HttpRequestException($"Erro ao chamar a API Gemini: {responseBody}");
        }
    }

    // Função simples para extrair palavras-chave (pode ser aprimorada)
    private string[] ExtractKeywords(string answer)
    {
        var words = answer.Split(' ')
                          .Where(word => word.Length > 4) // Pega palavras maiores que 4 letras
                          .Take(2) // Limita para 2 palavras
                          .ToArray();
        return words;
    }
}

// Classe para a requisição do usuário
public class UserRequest
{
    public string? Question { get; set; }
}

// Modelo para a resposta do Gemini
public class GeminiResponse
{
    public List<Candidate> Candidates { get; set; }  // Ajustado para ser uma lista
}

public class Candidate
{
    public Content? Content { get; set; }
    public string? FinishReason { get; set; }
}

public class Content
{
    public List<Part>? Parts { get; set; }  // Adicionada a propriedade Parts como uma lista
}

public class Part
{
    public string? Text { get; set; }
}
