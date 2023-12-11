using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;

namespace testOpenTelemetry.Util;

public class WeatherInstrumentation 
{

    //NET7 Example https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/examples/AspNetCore/Instrumentation.cs
    //internal const string ActivitySourceName = "Examples.AspNetCore";
    //internal const string MeterName = "Examples.AspNetCore";
    
     //Books meters
    private  Counter<int> FreezingDaysCounter { get; }
    private Counter<int> WeattherCallCounter { get; }
    public WeatherInstrumentation(IMeterFactory meterFactory, IConfiguration configuration)
    {
        //this.FreezingDaysCounter = this.meter.CreateCounter<long>("weather.days.freezing", "The number of days where the temperature is below freezing");

        // var meter = meterFactory.Create(configuration["BookStoreMeterName"] ?? 
        //                                     throw new NullReferenceException("BookStore meter missing a name"));
        ObservabilityOptions observabilityOptions = new();

        configuration
            .GetRequiredSection(nameof(ObservabilityOptions))
            .Bind(observabilityOptions);

        var meter = meterFactory.Create(observabilityOptions.ServiceName ?? throw new NullReferenceException("BookStore meter missing a name"));

        FreezingDaysCounter = meter.CreateCounter<int>("weather.days.freezing", "The number of days where the temperature is below freezing");
        WeattherCallCounter = meter.CreateCounter<int>("weather.calls", "The number of times the weather API is called");
    }
    
    public void FreezingDaysCount(int pCount) => this.FreezingDaysCounter.Add(pCount);
    public void WeatherCallCount() => this.WeattherCallCounter.Add(1);
}