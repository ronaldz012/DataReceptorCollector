namespace DataEmisor.Entities;
public class VehicleState
{
    public string Name { get; set; } = string.Empty;
    
    // Combustible
    public double Fuel { get; set; } = 100.0;  // % (0-100)
    
    // Motor
    public double Temp { get; set; } = 70.0;   // °C
    public double Rpm { get; set; } = 0;       // RPM
    public Enginestatus Status { get; set; } = 0; // 0=Off, 1=Idle, 2=Driving
    
    // Movimiento
    public double Speed { get; set; } = 0;     // km/h
    public double Lat { get; set; } = -17.9; 
    public double Lon { get; set; } = -67.1;
    public DateTime Timestamp { get; set; } =  DateTime.UtcNow;

    public enum Enginestatus
    {
        Off,
        Idle,
        Driving
    }
    
    // auxiliar variables
    private double _previousSpeed = 0;
    private DateTime _lastUpdate = DateTime.UtcNow;
    private double _targetSpeed = 0;
    private bool _isAccelerating = false;

    public void Update()
    {
        var rnd = Random.Shared;
        Timestamp = DateTime.UtcNow;
        var deltaTime = (Timestamp - _lastUpdate).TotalSeconds;
        if (deltaTime < 1) deltaTime = 1; //check how it behaves
        
        _lastUpdate = Timestamp;
        _previousSpeed = Speed;
        
        // ========================================
        // SIMULACIÓN DE COMPORTAMIENTO REALISTA
        // ========================================
        
        // 1. Decidir estado del motor (80% driving, 15% idle, 5% off)
        var action = rnd.Next(100);
        
        switch (action)
        {
            // 5% - Apagar motor
            case < 5:
                Status = Enginestatus.Off;
                _targetSpeed = 0;
                Rpm = 0;
                break;
            // 15% - stop for some reason but engine still works
            case < 20:
                Status = Enginestatus.Idle;
                _targetSpeed = 0;
                Rpm = 800 + rnd.Next(200); // RPM de ralentí
                break;
            // 80% - Driving (mos of the time)
            default:
            {
                Status = Enginestatus.Driving;
            
                // Decidir velocidad objetivo (realista)
                if (rnd.Next(100) < 10) // 10% chance de cambiar velocidad objetivo
                {
                    _targetSpeed = rnd.Next(0, 121); // 0-120 km/h
                }

                break;
            }
        }
        
        // 2. CALCULAR VELOCIDAD (con aceleración/frenado realista)
        if (Status == Enginestatus.Driving) // Conduciendo
        {
            if (Speed < _targetSpeed)
            {
                // Acelerando
                _isAccelerating = true;
                var acceleration = rnd.NextDouble() * 8 + 2; // 2-10 km/h por segundo
                
                // 🎯 MÉTRICA: Aceleración brusca (> 15 km/h/s)
                if (acceleration > 15)
                {
                    // do something here maybe ....
                }
                
                Speed += acceleration * deltaTime;
                if (Speed > _targetSpeed) Speed = _targetSpeed;
            }
            else if (Speed > _targetSpeed)
            {
                // Frenando
                _isAccelerating = false;
                var deceleration = rnd.NextDouble() * 10 + 3; // 3-13 km/h por segundo
                
                // 🎯 MÉTRICA: Frenada brusca (> 12 km/h/s)
                if (deceleration > 12)
                {
                    // Esto generará un evento de frenada brusca
                }
                
                Speed -= deceleration * deltaTime;
                if (Speed < _targetSpeed) Speed = _targetSpeed;
            }
            
            if (Speed < 0) Speed = 0;
            if (Speed > 150) Speed = 150;
            
            // RPM basado en velocidad (simplificado)
            Rpm = Speed * 50 + 800; // Ej: 60 km/h = 3800 RPM
            if (Rpm > 6000) Rpm = 6000;
        }
        else // Motor apagado o ralentí
        {
            // Desacelerar suavemente
            Speed -= 5 * deltaTime;
            if (Speed < 0) Speed = 0;
        }
        
        // 3. TEMPERATURA DEL MOTOR
        if (Status == Enginestatus.Off) // Motor apagado
        {
            // Enfriarse
            Temp -= rnd.NextDouble() * 2;
            if (Temp < 20) Temp = 20; // Temperatura ambiente
        }
        else if (Status == Enginestatus.Idle) // Ralentí
        {
            // Mantenerse caliente
            Temp += (rnd.NextDouble() - 0.5) * 1;
            if (Temp < 70) Temp = 70;
            if (Temp > 95) Temp = 95;
        }
        else // Conduciendo
        {
            // Calentarse según velocidad y RPM
            var heatFactor = (Speed / 100.0) + (Rpm / 5000.0);
            Temp += (rnd.NextDouble() * heatFactor * 2) - 0.5;
            
            //  MÉTRICA: Sobrecalentamiento
            if (Temp > 105)
            {
                // Evento de sobrecalentamiento (advertencia)
            }
            
            if (Temp < 70) Temp = 70;
            if (Temp > 115) Temp = 115; // Límite crítico
        }

        switch (Status)
        {
            // 4. CONSUMO DE COMBUSTIBLE
            // Motor apagado
            case Enginestatus.Off:
                // No consume
                break;
            // Ralentí
            case Enginestatus.Idle:
                // Consumo mínimo: ~0.8L/hora = 0.00022 L/segundo
                Fuel -= 0.02 * deltaTime;
                break;
            // Conduciendo
            default:
            {
                // Consumo basado en velocidad y aceleración
                var baseFuelConsumption = Speed / 1000.0; // Base según velocidad
            
                if (_isAccelerating)
                {
                    baseFuelConsumption *= 1.5; // +50% al acelerar
                }
            
                var rpmFactor = Rpm / 3000.0;
                baseFuelConsumption *= rpmFactor;
            
                Fuel -= baseFuelConsumption * deltaTime * rnd.NextDouble();
                break;
            }
        }
        
        // 🎯 MÉTRICA: Combustible bajo
        if (Fuel < 10)
        {
            // Alerta de combustible bajo
        }
        
        if (Fuel < 0) Fuel = 0;
        
        // 5. MOVIMIENTO (solo si está en marcha)
        if (Speed > 5) // Moviéndose
        {
            // Calcular distancia recorrida en este intervalo
            var distanceKm = (Speed * deltaTime) / 3600.0;
            
            // Mover en dirección aleatoria (simulación simple)
            var bearing = rnd.NextDouble() * 360; // Dirección en grados
            var bearingRad = bearing * Math.PI / 180.0;
            
            // Convertir distancia a cambio en coordenadas (aproximado)
            var latChange = (distanceKm / 111.0) * Math.Cos(bearingRad);
            var lonChange = (distanceKm / (111.0 * Math.Cos(Lat * Math.PI / 180.0))) * Math.Sin(bearingRad);
            
            Lat += latChange;
            Lon += lonChange;
        }
    }
    
    // 📊 Métodos helper para análisis
    public double GetSpeedChange()
    {
        return Speed - _previousSpeed;
    }
    
    public bool IsHarshBraking()
    {
        var speedDrop = _previousSpeed - Speed;
        return speedDrop > 12; // Más de 12 km/h de reducción
    }
    
    public bool IsHarshAcceleration()
    {
        var speedIncrease = Speed - _previousSpeed;
        return speedIncrease > 15; // Más de 15 km/h de aumento
    }
    
    public bool IsOverheating()
    {
        return Temp > 105;
    }
    
    public bool IsLowFuel()
    {
        return Fuel < 15;
    }
    
    public bool IsSpeeding()
    {
        return Speed > 120; // Límite de velocidad
    }
}