using Microsoft.EntityFrameworkCore;
using StockApp.Infra.IoC;
using DotNetEnv;
using StockApp.Infra.Data.Context;
using System;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Repositories;
using StockApp.Application.Services;
using Microsoft.AspNetCore.Builder;
using StockApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERRO: A ConnectionString 'DefaultConnection' não foi encontrada. Verifique appsettings.json ou .env.");
    throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureAPI(builder.Configuration);

builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ReviewService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Torna a classe Program acessível para testes de integração
public partial class Program { }
