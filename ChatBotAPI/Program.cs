using Microsoft.EntityFrameworkCore;                
using Microsoft.Extensions.DependencyInjection;     
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;               
using System.Text;                                  
using Microsoft.OpenApi.Models;                     
using ChatBot.Data;                                

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte ao HttpClientFactory
builder.Services.AddHttpClient();

// Configuração do banco de dados SQLite
builder.Services.AddDbContext<ChatBotContext>(options => 
    options.UseSqlite("Data Source=database.db")); // Use o SQLite como seu banco de dados

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração de Autenticação JWT (remova se não estiver usando)
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourissuer", // Substitua com seu emissor
        ValidAudience = "youraudience", // Substitua com sua audiência
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yoursecret")) // Substitua com sua chave secreta
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Exibe detalhes dos erros durante o desenvolvimento
    app.UseSwagger(); // Habilita o Swagger para documentação da API
    app.UseSwaggerUI(); // Interface do Swagger UI
}

// Ativar a política CORS
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection(); // Redireciona todas as requisições HTTP para HTTPS
app.UseAuthentication(); // Ativa a autenticação JWT
app.UseAuthorization(); // Ativa a autorização

app.MapControllers(); // Mapeia os controladores para as rotas

app.Run(); // Inicia o aplicativo
