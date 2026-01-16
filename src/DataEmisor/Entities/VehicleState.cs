namespace DataEmisor.Entities;

public class VehicleState
{
    public string Id { get; set; }
    public double Fuel { get; set; } = 100.0;  // liters
    public double Temp { get; set; } = 70.0; //C Celcius
    public double Lat { get; set; } = -17.9; 
    public double Lon { get; set; } = -67.1;
    
    //Break pressure? 
    //speed can be calculated with latitud and longitud
    //suggest other variables

    public void Update()
    {
        var rnd = Random.Shared;
        
        Fuel -= rnd.NextDouble() * 0.1; // reducing just a little
        if (Fuel < 0) Fuel = 0;

        Temp += rnd.NextDouble() * 2 - 0.9; // CHECK HOW IT Goes
        if (Temp < 70) Temp = 70;
        if (Temp > 115) Temp = 115;

        //position maybe a route later 
        Lat += (rnd.NextDouble() - 0.5) * 0.001;
        Lon += (rnd.NextDouble() - 0.5) * 0.001;
    }
}