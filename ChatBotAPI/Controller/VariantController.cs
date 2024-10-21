using Microsoft.AspNetCore.Mvc;       // Manipulação de requisições HTTP
using ChatBot.Models;              // Modelo de variantes (Variant)
using ChatBot.Data;                // Contexto do banco de dados
using System.Threading.Tasks;         // Para métodos assíncronos
using Microsoft.EntityFrameworkCore;  // Operações com o banco de dados


namespace ChatBot.Controller
{
    [ApiController]
[Route("api/[controller]")]
public class VariantController : ControllerBase
{
    private readonly ChatBotContext _context;

    public VariantController(ChatBotContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddVariant([FromBody] Variant newVariant)
    {
        _context.Variants.Add(newVariant);
        _context.SaveChanges();
        return Ok("Nova variante adicionada.");
    }

    [HttpGet]
    public IActionResult GetAllVariants()
    {
        var variants = _context.Variants.ToList();
        return Ok(variants);
    }
}

}