using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;  // Para definir o contexto do banco de dados
using ChatBot.Models;              // Para incluir os modelos no contexto


namespace ChatBot.Data
{
    public class ChatBotContext : DbContext
    {
        public DbSet<Response> Responses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<User> Users { get; set; }

        public ChatBotContext(DbContextOptions<ChatBotContext> options) : base(options) { }
    }
}