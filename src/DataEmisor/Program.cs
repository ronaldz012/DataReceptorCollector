// admin_setup.cs - Ejecutar una sola vez

using Microsoft.Extensions.Configuration;
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

builder.Logging.AddConsole();
var host = builder.Build();

host.Run();

// var factory = new ConnectionFactory 
// { 
//     HostName = "localhost", 
//     UserName = "admin",  // Usuario con permisos completos
//     Password = "admin_password" 
// };
//
// await using var connection = await factory.CreateConnectionAsync();
// await using var channel = await connection.CreateChannelAsync();
//
// await channel.ExchangeDeclareAsync("ventas.exchange", ExchangeType.Direct, durable: false);
// await channel.QueueDeclareAsync("ventas.pos", durable: true, exclusive: false, autoDelete: false);
// await channel.QueueBindAsync("ventas.pos", "ventas.exchange", "nueva.venta");
//
// Console.WriteLine("Mensaje enviado");