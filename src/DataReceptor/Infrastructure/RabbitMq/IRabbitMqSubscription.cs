using System.Text;
using DataReceptor.Application.Dtos;
using DataReceptor.Application.Services;
using DataReceptor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DataReceptor.Infrastructure.RabbitMq;

public interface IRabbitMqSubscription
{
    Task InitializeAsync();
}
public class RabbitMqSubscription(IServiceScopeFactory scopeFactory,IOptions<RabbitMqSettings> options) : IRabbitMqSubscription
{
    private RabbitMqSettings Settings = options.Value;
    private IConnection _connection;
    private IChannel _channel;
    private bool _initialized = false;
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var factory = new ConnectionFactory()
        {
            HostName = Settings.HostName,
            Port = Settings.Port,
            UserName = Settings.UserName,
            Password = Settings.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,  
            global: false
        );
        var consumer  = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var sender = scopeFactory.CreateAsyncScope().ServiceProvider.GetRequiredService<IMessageService>();

                CarTelemetryDto? carTelemetry  = System.Text.Json.JsonSerializer.Deserialize<CarTelemetryDto>(ea.Body.ToArray());
                
                if (carTelemetry == null)  await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                else await sender.SaveCarTelemetry(carTelemetry);
                
                await Task.Yield();
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag, 
                    multiple: false, 
                    requeue: false  
                );
            }
        };
        
        _channel.BasicConsumeAsync(queue: Settings.Queue.Name, autoAck: false, consumer: consumer);
        _initialized = true;

    }
}
