using Microsoft.AspNetCore.Mvc; // Para manipulação de rotas e respostas HTTP
using System.Threading.Tasks; // Para métodos assíncronos (async/await)
using ChatBot.Data; // Para acessar o contexto do banco de dados (ChatBotContext)
using ChatBot.Models; // Para usar os modelos (Response, etc.)
using Microsoft.EntityFrameworkCore; // Para operações com o banco de dados (SQLite)
using System.Net.Http; // Para uso do HttpClient
using System.Net.Http.Headers; // Para configurar cabeçalhos HTTP
using Newtonsoft.Json; // Para serialização JSON
using System.Text; // Para codificação UTF-8

namespace ChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly ChatBotContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ResponseController(ChatBotContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory; // Injetando o IHttpClientFactory
        }

        // Método para comunicar-se com a API externa do OpenAI
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return BadRequest("Mensagem inválida.");
            }

            // Crie um cliente HTTP usando o HttpClientFactory
            var httpClient = _httpClientFactory.CreateClient();

            // Configure o cliente com o seu API Key
            var apiKey = ""; // Substitua pelo seu API Key
            var requestUrl = "https://api.openai.com/v1/chat/completions"; // Atualize para a URL correta da API do OpenAI

            // Configure os headers
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Crie o payload
            var payload = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                },
                temperature = 1,
                max_tokens = 2048,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0,
                response_format = new { type = "text" }
            };

            // Serializa o payload para JSON
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Envie a solicitação para a API externa
            var response = await httpClient.PostAsync(requestUrl, content);

            // Verifique se a solicitação foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                
                // Assumindo que o resultado da resposta está em "choices" e "content"
                string botReply = jsonResponse.choices[0].message.content;
                return Ok(new { text = botReply }); // Retorna a resposta do bot
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Erro ao comunicar com a API externa.");
            }
        }
    }
}
