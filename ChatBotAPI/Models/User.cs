using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    // Para definir as propriedades e validações dos modelos
using System.ComponentModel.DataAnnotations.Schema;  // Para mapeamento de tabelas e chaves estrangeiras

namespace ChatBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; } // "admin" or "user"
    }
}