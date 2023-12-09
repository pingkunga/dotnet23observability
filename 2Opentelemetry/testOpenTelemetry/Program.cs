using System;
using testOpenTelemetry.Util;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//                      .AddJsonFile("appsettings.json", false, true)
//                      .AddEnvironmentVariables()
//                      .AddCommandLine(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddObservability();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//PING
app.UseTraceIdResponseHeader();
app.MapGet("/ping", () => "pong");

app.MapControllers();

app.Run();
