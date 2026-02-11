using DataReceptor.Infrastructure.RabbitMq;
using Microsoft.Extensions.Hosting;

namespace DataReceptor.Application.Services;

public class RabbitMqWorker(IRabbitMqSubscription rabbitMqSubscription) : BackgroundService
{
    protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await rabbitMqSubscription.InitializeAsync();
        await Task.Delay (Timeout.Infinite, stoppingToken);
    }
}