using AutoMapper;
using DataReceptor.Application.Dtos;
using DataReceptor.Domain.Entities;

namespace DataReceptor.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CarTelemetryDto, CarTelemetry>();
    }
}