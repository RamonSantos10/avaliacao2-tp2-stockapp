using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockApp.Application.Interfaces;
using StockApp.Application.Mappings;
using StockApp.Application.Services;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using StockApp.Infra.Data.EntityConfiguration;
using StockApp.Infra.Data.Identity;
using StockApp.Infra.Data.Repositories;
using System;

namespace StockApp.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IInventoryService, InventoryService>();
            
            // Employee and Employee Evaluation Services
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeEvaluationRepository, EmployeeEvaluationRepository>();
            services.AddScoped<IEmployeePerformanceEvaluationService, EmployeePerformanceEvaluationService>();

            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IAnonymousFeedbackRepository, AnonymousFeedbackRepository>();
            services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ReviewService>();
            
            // Anonymous Feedback Service
            services.AddScoped<IAnonymousFeedbackService, AnonymousFeedbackService>();
            
            // Webhook Service
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddHttpClient<WebhookService>();

            var myhandlers = AppDomain.CurrentDomain.Load("StockApp.Application");
            services.AddMediatR(myhandlers);

            return services;
        }
    }
}
