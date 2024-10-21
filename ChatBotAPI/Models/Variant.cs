using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    // Para definir as propriedades e validações dos modelos
using System.ComponentModel.DataAnnotations.Schema;  // Para mapeamento de tabelas e chaves estrangeiras


namespace ChatBot.Models
{
    public class Variant
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}