// admin_setup.cs - Ejecutar una sola vez

using DataEmisor.Infrastructure;
using DataEmisor.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;


var builder = Host.CreateApplicationBuilder(args);

// Configurar appsettings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Logging.AddConsole();
var host = builder.Build();
var rabbitConnection = host.Services.GetRequiredService<IRabbitMqConnectionLegacy>();
await rabbitConnection.InitializeAsync();
Console.WriteLine("All GOOD :D");

host.Run();
