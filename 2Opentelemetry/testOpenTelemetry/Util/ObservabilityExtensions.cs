using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace testOpenTelemetry.Util;
public static class ObservabilityRegistration
{
    public static ActivitySource ActivitySource = null;
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;

        // This is required if the collector doesn't expose an https endpoint. By default, .NET
        // only allows http2 (required for gRPC) to secure endpoints.
        //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var configuration = builder.Configuration;

        ObservabilityOptions observabilityOptions = new();

        configuration
            .GetRequiredSection(nameof(ObservabilityOptions))
            .Bind(observabilityOptions);
        
        ActivitySource = new ActivitySource(observabilityOptions.ServiceName);

        builder.Host.AddSerilog();

        builder.Services
                .AddOpenTelemetry()
                .AddTracing(observabilityOptions)
                .AddMetrics(observabilityOptions);

        return builder;
    }

    private static OpenTelemetryBuilder AddTracing(this OpenTelemetryBuilder builder, ObservabilityOptions observabilityOptions)
    {
        builder.WithTracing(tracing =>
        {
            tracing
                .AddSource(observabilityOptions.ServiceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(observabilityOptions.ServiceName))
                .SetErrorStatusOnException()
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation(options =>
                {
                    //PING ASPNETCORE GRPC SUPPORT
                    //options.EnableGrpcAspNetCoreSupport = true;
                    options.RecordException = true;
                })
                .AddHttpClientInstrumentation();
                
                //https://www.nuget.org/packages/Npgsql.OpenTelemetry#readme-body-tab
                // This activates up Npgsql's tracing:
                //.AddNpgsql()

           
            tracing
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = observabilityOptions.CollectorUri;
                    options.ExportProcessorType = ExportProcessorType.Batch;
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                });
        });

        return builder;
    }

    private static OpenTelemetryBuilder AddMetrics(this OpenTelemetryBuilder builder, ObservabilityOptions observabilityOptions)
    {
        builder.WithMetrics(metrics =>
        {
            //var meter = new Meter(observabilityOptions.ServiceName);

            metrics
                .AddMeter(observabilityOptions.ServiceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(observabilityOptions.ServiceName))
                .AddAspNetCoreInstrumentation();

                /*
                //AddAspNetCoreInstrumentation = กลุ่มข้างล่าง
                //Internal ASPNETCORE METER
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("Microsoft.AspNetCore.Http.Connections")
                .AddMeter("Microsoft.AspNetCore.Routing")
                .AddMeter("Microsoft.AspNetCore.Diagnostics")
                .AddMeter("Microsoft.AspNetCore.RateLimiting");
                */
                
            metrics
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = observabilityOptions.CollectorUri;
                    options.ExportProcessorType = ExportProcessorType.Batch;
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                });
        });

        return builder;
    }

    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder
            .UseSerilog((context, provider, options) =>
            {
                var environment = context.HostingEnvironment.EnvironmentName;
                var configuration = context.Configuration;

                ObservabilityOptions observabilityOptions = new();

                configuration
                    .GetSection(nameof(ObservabilityOptions))
                    .Bind(observabilityOptions);

                var serilogSection = $"{nameof(ObservabilityOptions)}:Serilog";

                options
                    .ReadFrom.Configuration(context.Configuration.GetRequiredSection(serilogSection))
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironment(environment)
                    .Enrich.WithProperty("ApplicationName", observabilityOptions.ServiceName)
                    .WriteTo.Console();

                /*
                builder.Host.UseSerilog((context, services, configuration) => configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext());  

                */

                options.WriteTo.OpenTelemetry(cfg =>
                {
                    cfg.Endpoint = $"{observabilityOptions.CollectorUrl}/v1/logs";
                    cfg.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                    cfg.ResourceAttributes = new Dictionary<string, object>
                                                {
                                                    {"service.name", observabilityOptions.ServiceName},
                                                    {"service.version", typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"},
                                                    {"service.InstanceId", Environment.MachineName },
                                                    {"flag", true},
                                                    {"value", 3.14}
                                                };
                });

               
            });
        return hostBuilder;
    }



}