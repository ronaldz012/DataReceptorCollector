using DataEmisor.Entities;
using DataEmisor.Infrastructure.RabbitMQ;
using DataEmisor.UseCases.MessageSender;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataEmisor.UseCases.CarSimulator;

public class CarSimulatorWorker(ILogger<CarSimulatorWorker> logger, SenderService sender, IRabbitMqConnection rabbitMqConnection) : BackgroundService
{
    private readonly string[] _targetCars = ["AUTO-PORSCHE-01", "AUTO-TESLA-02", "AUTO-TOYOTA-03", "AUTO-VOLVO-04"];
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Iniciando infraestructura de RabbitMQ...");
        await rabbitMqConnection.InitializeAsync();
        logger.LogInformation("Arrancando simulación para {Count} vehículos...", _targetCars.Length);
        var tasks = _targetCars.Select(id => SimulateVehicleAsync(id, stoppingToken));
        await Task.WhenAll(tasks);
    }

    private  async Task SimulateVehicleAsync(string carName, CancellationToken stoppingToken)
    {
        var state = new VehicleState() { Name = carName };
        logger.LogInformation("Simulación iniciada para: {CarId}", carName);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                state.Update();

                await sender.SendMessage(state);
                await Task.Delay(Random.Shared.Next(1000, 3000), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en el ciclo de simulación de {CarId}", carName);
                await Task.Delay(5000, stoppingToken); 
            }
        }
    }
}