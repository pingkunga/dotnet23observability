using System.Diagnostics;
using Grpc.Net.Client.Balancer;
using Microsoft.AspNetCore.Mvc;
using testOpenTelemetry.Util;
namespace testOpenTelemetry.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherInstrumentation _instrumentation;
    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherInstrumentation instrumentation)
    {
        _logger = logger;
        _instrumentation = instrumentation;
        _logger.LogInformation("WeatherForecast controller called ");
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using var activity = ObservabilityRegistration.ActivitySource.StartActivity("WeatherForecastController.Get");
        {
            _logger.LogInformation("WeatherForecast get method Starting.");

            var foecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            
            // Optional: Count the freezing days
            _instrumentation.FreezingDaysCount (foecast.Count(f => f.TemperatureC < 0));
            _instrumentation.WeatherCallCount();
            DoSomeThing();
            return foecast;
        }
    }

    private void DoSomeThing()
    {
        _logger.LogInformation("WeatherForecast DoSomeThing");
    }
}
