using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    // Para definir as propriedades e validações dos modelos
using System.ComponentModel.DataAnnotations.Schema;  // Para mapeamento de tabelas e chaves estrangeiras
public class Response
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public string? Keyword { get; set; }
    public string? Variant { get; set; }

    public string? Question { get; set; }
    [NotMapped]
    public object? Answer { get; internal set; }
    public bool IsApproved { get; set; } = false; // Initially false, requires admin approval
}