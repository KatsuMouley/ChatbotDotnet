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

// Add services to the container.
builder.Services.AddDbContext<ChatBotContext>(options => options.UseSqlite("Data Source=chatbot.db"));
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

// Remova esta parte se não estiver usando JWT
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourissuer",
        ValidAudience = "youraudience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yoursecret"))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ativar a política CORS
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
