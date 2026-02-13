namespace DataReceptor.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public required string Name {get; set;} = string.Empty; // most be unique
    
    public double MaxTemp { get; set; } // Celcius
    public double minFuel { get; set; } // 0 -100 %
    
    public ICollection<CarTelemetry>   CarTelemetries { get; set; } = new List<CarTelemetry>();
}