using Microsoft.AspNetCore.Mvc;       // Manipulação de rotas HTTP
using ChatBot.Models;                // Modelo de denúncia (Report)
using ChatBot.Data;                  // Contexto do banco de dados (ChatBotContext)
using Microsoft.EntityFrameworkCore; // Operações com o banco de dados

namespace ChatBot.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ChatBotContext _context;

        public ReportController(ChatBotContext context)
        {
            _context = context;
        }

        // Endpoint para cadastrar uma denúncia
        [HttpPost]
        public async Task<IActionResult> ReportResponse([FromBody] Report report)
        {
            if (report == null || string.IsNullOrWhiteSpace(report.Message))
            {
                return BadRequest("Dados da denúncia inválidos.");
            }

            // report.ReportedAt = DateTime.UtcNow; // Definindo a data de registro
            report.ReportedAt = DateTime.UtcNow.AddDays(-6); // Definindo a data de registro


            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return Ok("Denúncia registrada com sucesso.");
        }
    }
}
