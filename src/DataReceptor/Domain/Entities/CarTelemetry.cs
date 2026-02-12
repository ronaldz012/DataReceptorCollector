namespace DataReceptor.Domain.Entities;

public class CarTelemetry
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public double Fuel { get; set; } 
    public double Temp { get; set; }
    public double Lat { get; set; } 
    public double Lon { get; set; }

    public DateTime? DeletedAt = null;
    
    public Car Car { get; set; } = default!;
}