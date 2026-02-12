using System.Security;
using AutoMapper;
using DataReceptor.Application.Dtos;
using DataReceptor.Domain.Entities;
using DataReceptor.Infrastructure.Persistence;

namespace DataReceptor.Application.Services;

public class IMessageService(DataContext context, IMapper mapper)
{
    public async Task<bool> SaveCarTelemetry(  CarTelemetryDto carTelemetryDto)
    {
        context.CarTelemetry.Add(mapper.Map<CarTelemetry>(carTelemetryDto));
        await context.SaveChangesAsync();
        return true;
    }
}