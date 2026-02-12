using System.Text;
using System.Text.Json;
using DataEmisor.Infrastructure.RabbitMQ;
using RabbitMQ.Client;

namespace DataEmisor.UseCases.MessageSender;

public class SenderService(IRabbitMqConnection rabbitConnection)
{

    public async Task SendMessage<T>(T content)
    {
        // 1. Obtener un canal del pool (esperará si los 10 están ocupados)
        var channel = await rabbitConnection.GetChannelFromPool();

        try
        {
            // 2. Preparar el mensaje
            var json = JsonSerializer.Serialize(content);
            var body = Encoding.UTF8.GetBytes(json);

            // 3. Publicar usando los settings centralizados
            await channel.BasicPublishAsync(
                exchange: rabbitConnection.Settings.Exchange.Name,
                routingKey: rabbitConnection.Settings.RoutingKey,
                mandatory: true,
                body: body);
            Console.WriteLine($"Enviando información: {json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar mensaje: {ex.Message}");
            throw;
        }
        finally
        {
            rabbitConnection.ReturnChannelToPool(channel);
        }
    }
}