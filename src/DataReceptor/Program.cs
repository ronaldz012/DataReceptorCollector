
using DataReceptor.Application.Services;
using DataReceptor.Infrastructure.Persistence;
using DataReceptor.Infrastructure.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configurar appsettings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Logging.AddConsole();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ReceptorConnection")));

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
builder.Services.AddSingleton<IRabbitMqSubscription, RabbitMqSubscription>();
builder.Services.AddHostedService<RabbitMqWorker>();
var host = builder.Build();
Console.WriteLine("All GOOD :D");
host.Run();