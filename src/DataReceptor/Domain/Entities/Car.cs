namespace DataReceptor.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public required string Name {get; set;} = string.Empty; // most be unique
    
    public ICollection<CarTelemetry>   CarTelemetries { get; set; } = new List<CarTelemetry>();
}