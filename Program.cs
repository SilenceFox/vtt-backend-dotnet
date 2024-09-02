using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Requests.Utility;
using Database;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

	////////////////////////////////////////////////////////////////
	// Testing the "Database" Operations, development only	      //
	////////////////////////////////////////////////////////////////

	// using Database <--- From this module 
        LogEntry smolLog = new LogEntry("thisIsAType", "Very Important Message");
        LocalData.SaveLog("file1", smolLog);
        LocalData.SaveLog("file2", smolLog);
        LocalData.SaveLog("file2", smolLog);

	////////////////////////////////////////////////////////////////
						     ////// END	      //
							////////////////

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/joao", () => PingRequest.Response());

        Requests.Dices.API.AddRequests(app);

        app.MapGet("/weatherforecast", () =>
        {
            WeatherForecast[]? forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();


        app.Run();
    }

    // WeatherForecast record definition
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
