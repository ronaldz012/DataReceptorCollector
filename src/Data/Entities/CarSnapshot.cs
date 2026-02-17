namespace Data.Entities;

public class CarSnapshot
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public double Fuel { get; set; } 
    public double Temp { get; set; }
    public double Lat { get; set; } 
    public double Lon { get; set; }
    public Car Car { get; set; } = default!;
}