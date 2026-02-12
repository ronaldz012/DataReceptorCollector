using System.Security;
using AutoMapper;
using DataReceptor.Application.Dtos;
using DataReceptor.Domain.Entities;
using DataReceptor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DataReceptor.Application.Services;

public class IMessageService(DataContext context, IMapper mapper)
{
    public async Task<bool> SaveCarTelemetry(  CarTelemetryDto carTelemetryDto)
    {
        var car = await context.Cars.FirstOrDefaultAsync(c => c.Name.Equals(carTelemetryDto.Name)) ?? new Car(){Name = carTelemetryDto.Name};
        var newTelemetry = mapper.Map<CarTelemetry>(carTelemetryDto);
        newTelemetry.Car= car;
        context.CarTelemetry.Add(newTelemetry);
        await context.SaveChangesAsync();
        Console.WriteLine("data saved");
        await Task.Delay(1000);
        return true;
    }
}