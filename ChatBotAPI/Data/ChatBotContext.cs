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
        public ChatBotContext(DbContextOptions<ChatBotContext> options) : base(options) { }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Variant> Variants { get; set; }

        // Adicione outros DbSet para suas outras entidades, se necessário

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure outras propriedades do modelo, se necessário
        }
    }
}
