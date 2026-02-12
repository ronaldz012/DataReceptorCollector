using DataEmisor.Infrastructure.RabbitMQ;
using DataEmisor.UseCases.CarSimulator;
using DataEmisor.UseCases.MessageSender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataEmisor.Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        services.AddSingleton<SenderService>();
        services.AddHostedService<CarSimulatorWorker>();
        return services;
    }
}