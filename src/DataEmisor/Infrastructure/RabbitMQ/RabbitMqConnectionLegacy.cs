using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DataEmisor.Infrastructure.RabbitMQ;

public interface IRabbitMqConnection : IAsyncDisposable
{
    Task InitializeAsync();
    IChannel GetChannel();
    RabbitMqSettings Settings { get; }
}

public class RabbitMqConnectionLegacy : IRabbitMqConnection
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<IRabbitMqConnection> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _initialized = false;

    public RabbitMqConnectionLegacy(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConnectionLegacy> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }
    
    public RabbitMqSettings Settings => _settings;
    
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        var factory = new ConnectionFactory //connectin settings
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync( 
            exchange: _settings.Exchange.Name,
            type: _settings.Exchange.Type,
            durable: _settings.Exchange.Durable,
            autoDelete: false);

        await _channel.QueueDeclareAsync(
            queue: _settings.Queue.Name,
            durable: _settings.Queue.Durable,
            exclusive: _settings.Queue.Exclusive,
            autoDelete: _settings.Queue.AutoDelete);

        await _channel.QueueBindAsync( // direct binding 
            queue: _settings.Queue.Name,
            exchange: _settings.Exchange.Name,
            routingKey: _settings.RoutingKey);

        _initialized = true;
        _logger.LogInformation("Rabbit Mq  Initialized :D");
    }

    public IChannel GetChannel()
    {
        if (_channel == null || !_initialized)
            throw new InvalidOperationException("RabbitMQ no inicializado");
        return _channel;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}