using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Data;
using ChatBot.Models;
using Microsoft.EntityFrameworkCore;  // Operações com o banco de dados
using Microsoft.AspNetCore.Authorization; // Para autenticação e autorização

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ChatBotContext _context;
    private readonly string _apiKey = "";  // Insira GEMINI API KEY

    public ChatController(HttpClient httpClient, ChatBotContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

        [HttpPost("InsertAsk")]
        public async Task<ActionResult<Question>> PostQuestion([FromBody] Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostQuestion), new { id = question.Id }, question);
        }

        [HttpPost("InsertAnswer")]
        public async Task<ActionResult<Answer>> PostAnswer([FromBody] Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostAnswer), new { id = answer.Id }, answer);
        }

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] UserRequest request)
    {
        try
        {
            // Call the Gemini API and get the raw response
            var geminiResponse = await SendMessageToGeminiAsync(request.Question);

            // Return the response from the Gemini API directly
            return Ok(geminiResponse);
        }
        catch (HttpRequestException ex)
        {
            // Return an error in case of a failure to call the API
            return BadRequest(new { error = ex.Message });
        }
    }

    // Function to call the Gemini API
    private async Task<string> SendMessageToGeminiAsync(string userMessage)
    {
        var geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey;

        // Request configuration for the Gemini API
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[] { new { text = "Responda a frase " + userMessage + " com no máximo 300 caracteres" } }
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

        // Make the POST request to the Gemini API
        var response = await _httpClient.PostAsync(geminiEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return responseBody;  // Return the response from Gemini
        }
        else
        {
            throw new HttpRequestException($"Error calling the Gemini API: {responseBody}");
        }
    }


    //Get endpoints
    // Endpoint para obter uma pergunta pelo ID como string
    [HttpGet("questions/{id}")]
    public async Task<ActionResult<string>> GetQuestionContentById(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
        {
            return NotFound(); // Retorna 404 se a pergunta não for encontrada
        }

        return Ok(question.Content); // Retorna o conteúdo da pergunta como string
    }

    // Endpoint para obter uma resposta pelo ID como string
    [HttpGet("answers/{id}")]
    public async Task<ActionResult<string>> GetAnswerContentById(int id)
    {
        var answer = await _context.Answers.FindAsync(id);

        if (answer == null)
        {
            return NotFound(); // Retorna 404 se a resposta não for encontrada
        }

        return Ok(answer.Content); // Retorna o conteúdo da resposta como string
    }

}

// Class for user request
public class UserRequest
{
    public int Id { get; set; }
    public string? Question { get; set; }
}

// Class for saving answers
public class AnswerRequest
{
    public string? Answer { get; set; }
}

// Other model classes remain unchanged

// Candidate.cs
public class Candidate
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? FinishReason { get; set; }
}

// Response.cs (Modelo fictício, caso precise)
public class Response
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Report.cs
public class Report
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public DateTime ReportedAt { get; set; }
}

// Variant.cs
public class Variant
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

// User.cs
public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
}
