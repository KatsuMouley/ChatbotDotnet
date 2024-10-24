using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    // Para definir as propriedades e validações dos modelos
using System.ComponentModel.DataAnnotations.Schema;  // Para mapeamento de tabelas e chaves estrangeiras

namespace ChatBot.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string? Content { get; set; } // Adicione outras propriedades conforme necessário
    }

    public class Answer
    {
        public int Id { get; set; }
        public string? Content { get; set; } // Adicione outras propriedades conforme necessário
    }

    public class Report
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }  // Relacionamento com Answer
        public string? Message { get; set; }
        public DateTime? ReportedAt { get; set; }
    }


    public class Variant
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}

