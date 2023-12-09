using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger(); // <-- Change this line!
try
{
    Log.Information("Starting up!");

    var builder = WebApplication.CreateBuilder(args);

    //Add Serilog
    // Set up logging with Serilog
    // https://github.com/dotnet/aspnetcore/issues/48437
    //builder.Logging.ClearProviders();
    // var logger = new LoggerConfiguration()
    //     .ReadFrom.Configuration(builder.Configuration)
    //     .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    //     .MinimumLevel.Override("Microsoft.AspNetCore",
    //         Enum.Parse<LogEventLevel>(builder.Configuration["AspNetCoreLogLevel"] ?? "Information"))
    //     .Enrich.FromLogContext()
    //     .CreateLogger();

    // Log.Logger = logger;
    //builder.Logging.AddSerilog(logger, true);

    //Add support to logging with SERILOG
    // builder.Host.UseSerilog((context, configuration) =>
    //     configuration.ReadFrom.Configuration(context.Configuration));  

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());     

    //builder.Host.UseSerilog();
    // Add services to the container.

    builder.Services.AddControllers();
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
    //Add Serilog
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
