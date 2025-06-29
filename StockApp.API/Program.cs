using StockApp.Infra.IoC;
using DotNetEnv; // Adicione este using para o DotNetEnv

internal class Program
{
    private static void Main(string[] args)
    {
        
        Env.Load(); 

        var builder = WebApplication.CreateBuilder(args);

       
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddInfrastructureAPI(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
    }
}