using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Apenas este construtor deve existir.
        public ResponseController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Endpoint para gerar a imagem com base no input do usuário
        [HttpGet("generate-image")]
        public async Task<IActionResult> GenerateImage([FromQuery] string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return BadRequest("Prompt is required.");
            }

            // URL da API externa
            var apiUrl = $"https://image.pollinations.ai/prompt/{prompt}";

            try
            {
                // Faz a requisição à API externa para gerar a imagem
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error retrieving the image.");
                }

                // Lê a imagem como array de bytes
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                // Retorna a imagem como FileResult
                return File(imageBytes, "image/jpeg"); // ou "image/png" dependendo do formato
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error while calling the external service: {e.Message}");
            }
        }
    }
}
