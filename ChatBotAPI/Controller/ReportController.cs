using Microsoft.AspNetCore.Mvc;       // Manipulação de rotas HTTP
using ChatBot.Models;              // Modelo de denúncia (Report)
using ChatBot.Data;                // Contexto do banco de dados (ChatBotContext)
using Microsoft.EntityFrameworkCore;  // Operações com o banco de dados
using Microsoft.AspNetCore.Authorization; // Para autenticação e autorização

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

    [HttpPost]
    public IActionResult ReportResponse([FromBody] Report report)
    {
        _context.Reports.Add(report);
        _context.SaveChanges();
        return Ok("Denúncia registrada.");
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteResponse(int id)
    {
        var response = _context.Responses.Find(id);
        if (response == null)
            return NotFound();

        _context.Responses.Remove(response);
        _context.SaveChanges();
        return Ok("Resposta removida.");
    }

    [Authorize(Roles = "admin")]
    [HttpPut("approve/{id}")]
    public IActionResult ApproveResponse(int id)
    {
        var response = _context.Responses.Find(id);
        if (response == null)
            return NotFound();

        response.IsApproved = true;
        _context.SaveChanges();
        return Ok("Resposta aprovada.");
    }
}

}