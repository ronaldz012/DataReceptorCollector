namespace DataReceptor.Infrastructure.RabbitMq;

public class RabbitMqSettings
{
    public  const string SectionName = "RabbitMq";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public ExchangeSettings Exchange { get; set; } = new();
    public QueueSettings Queue { get; set; } = new();
    public string RoutingKey { get; set; } = string.Empty;
    
}

public class ExchangeSettings
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "direct";
    public bool Durable { get; set; } = true;
}

public class QueueSettings
{
    public string Name { get; set; } = string.Empty;
    public bool Durable { get; set; } = false;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
}
