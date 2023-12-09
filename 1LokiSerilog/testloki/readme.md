dotnet new webapi -n testloki

dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Loki  ใช้จริง

dotnet add package Serilog.Sinks.Grafana.Loki

dotnet run --launch-profile https



https://github.com/josephwoodward/Serilog-Sinks-Loki
https://www.nuget.org/packages/Serilog.AspNetCore/#readme-body-tab
Failed to determine the https port for redirect.

Good

* https://github.com/serilog/serilog-aspnetcore?source=post_page-----5cba1d0dea2--------------------------------#two-stage-initialization

Reading List
* https://procodeguide.com/programming/aspnet-core-logging-with-serilog/

KM
* https://medium.com/@brucycenteio/adding-serilog-to-asp-net-core-net-7-8-5cba1d0dea2
* https://serilog.net/
* https://betterstack.com/community/guides/logging/how-to-start-logging-with-serilog/
* https://medium.com/c-sharp-progarmming/net-core-microservice-logging-with-grafana-and-loki-92cd2783ed88'
** https://github.com/arkapravasinha/LokiGraf/tree/master/LokiGraf.API
* https://code-maze.com/structured-logging-in-asp-net-core-with-serilog/

* https://www.c-sharpcorner.com/article/structured-logging-using-serilog-in-asp-net-core-7-0/

Error UseSerilog program.cs dotnet run stuck in building
* https://stackoverflow.com/questions/64966582/useserilog-program-cs-dotnet-run-stuck-in-building
* https://stackoverflow.com/questions/62903160/serilog-always-stuck-at-when-app-starting

Docker
* https://github.com/the-cloud-camp/observability-week