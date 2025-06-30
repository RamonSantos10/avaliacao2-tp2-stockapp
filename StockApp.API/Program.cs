using Microsoft.EntityFrameworkCore;
using StockApp.Infra.IoC;
using StockApp.Infra.Data.Context;
using System;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Repositories;
using StockApp.Application.Services;

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddInfrastructureAPI(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        builder.Services.AddScoped<IFeedbackService, FeedbackService>();
        builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
        builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
