# OpenTelemetry


## Start

```
dotnet new sln -o testOpenTelemetry

dotnet new webapi -n testOpenTelemetry

dotnet sln testOpenTelemetry.sln add .\testOpenTelemetry\testOpenTelemetry.csproj 

```

## Package 

* OpenTelemetry: Traces & Metrics

```
#dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol --version 1.6.0
dotnet add package OpenTelemetry.Extensions.Hosting --version 1.6.0
dotnet add package OpenTelemetry.Instrumentation.AspNetCore --version 1.6.0-rc.1

//Enables HTTP Instrumentation. App1 makes an HTTP request to App2, if we want to trace the HTTP call between these 2 apps we can do it simply by adding this extension method.
dotnet add package OpenTelemetry.Instrumentation.Http --version 1.6.0-rc.1
dotnet add package OpenTelemetry.Instrumentation.SqlClient --version 1.6.0-rc.1
dotnet add package Npgsql.OpenTelemetry --version 8.0.1

```

* OpenTelemetry: Logs

```
dotnet add package Serilog.AspNetCore --version 7.0.0
dotnet add package Serilog.Enrichers.Context --version 4.6.5
dotnet add package Serilog.Sinks.OpenTelemetry --version 1.2.0
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol --version 1.6.0
```

## Register OpenTelemetry Program.cs

* Logs

* Trace

* Metrics


## References

* https://dev.to/kim-ch/observability-net-opentelemetry-collector-25g1