using AutoMapper;
using Data.Entities;
using DataReceptor.Application.Dtos;

namespace DataReceptor.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CarTelemetryDto, CarTelemetry>();
    }
}