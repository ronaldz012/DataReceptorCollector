using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DataEmisor.Infrastructure.RabbitMQ;

public interface IRabbitMqConnection : IAsyncDisposable
{
    Task InitializeAsync();
    Task<IChannel> GetChannelFromPool();
    void ReturnChannelToPool(IChannel channel);
    RabbitMqSettings Settings { get; }
    
}

public class RabbitMeConnection(IOptions<RabbitMqSettings> settings, ILogger<RabbitMeConnection> logger) : IRabbitMqConnection
{
    
    private IConnection? _connection;
    
    private readonly ConcurrentStack<IChannel> _channelPool = new();

    private const int PoolSize = 10;
    private bool _initialized = false;
    private readonly SemaphoreSlim _poolSemaphore = new SemaphoreSlim(PoolSize, PoolSize);

    public  RabbitMqSettings Settings => settings.Value;

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var factory = new ConnectionFactory {
            HostName = Settings.HostName,
            Port = Settings.Port,
            UserName = Settings.UserName,
            Password = Settings.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync();

        // Configuración inicial de colas/exchanges
        using (var setupChannel = await _connection.CreateChannelAsync())
        {
            await setupChannel.ExchangeDeclareAsync(Settings.Exchange.Name, Settings.Exchange.Type, Settings.Exchange.Durable);
            await setupChannel.QueueDeclareAsync(Settings.Queue.Name, Settings.Queue.Durable, Settings.Queue.Exclusive, Settings.Queue.AutoDelete);
            await setupChannel.QueueBindAsync(Settings.Queue.Name, Settings.Exchange.Name, Settings.RoutingKey);
        }

        // Llenamos el pool
        for (int i = 0; i < PoolSize; i++)
        {
            _channelPool.Push(await _connection.CreateChannelAsync());
        }

        _initialized = true;
    }

    public async Task<IChannel> GetChannelFromPool()
    {
        await _poolSemaphore.WaitAsync(); 
        logger.LogInformation($"Disposable channels{_channelPool.Count}");
        if (_channelPool.TryPop(out var channel))
        {
            if (channel.IsOpen) return channel;
            return await _connection!.CreateChannelAsync();
        }

        _poolSemaphore.Release();
        throw new Exception("Error crítico: No se pudo obtener canal del pool.");
    }
    public void ReturnChannelToPool(IChannel channel)
    {
        if (channel.IsOpen)
        {
            _channelPool.Push(channel);
        }
        else
        {
            var newChannel = _connection!.CreateChannelAsync().GetAwaiter().GetResult();
            _channelPool.Push(newChannel);
        }
        _poolSemaphore.Release();
    }


    public async ValueTask DisposeAsync()
    {
        logger.LogInformation("Cleaning RabbitMQ connection");

        while (_channelPool.TryPop(out var channel))
        {
            try 
            {
                if (channel.IsOpen)
                {
                    await channel.CloseAsync();
                }
                await channel.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error closing channel Details: {ex.Message}");
            }
        }

        if (_connection != null)
        {
            try 
            {
                if (_connection.IsOpen)
                {
                    await _connection.CloseAsync();
                }
                await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error closing the main Connection {ex.Message}");
            }
        }
        _poolSemaphore.Dispose();
    }
}