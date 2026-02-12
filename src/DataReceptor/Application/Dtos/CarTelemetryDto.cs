namespace DataReceptor.Application.Dtos;

public class CarTelemetryDto
{
    public required string Name { get; set; } = string.Empty;
    public double Fuel { get; set; } 
    public double Temp { get; set; }
    public double Lat { get; set; } 
    public double Lon { get; set; } 
}