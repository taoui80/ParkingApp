using System;

namespace ParkingApp.models;

public class ParkingSession
{
    public int Id { get; set; }
    public string? CarLicensePlate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set;} 
    
    public ParkingSession(int id, string? carLicensePlate, DateTime startTime)
    {
        Id = id;
        CarLicensePlate = carLicensePlate ?? throw new ArgumentNullException(nameof(carLicensePlate));
        StartTime = startTime;
        EndTime = null;   
    }

    public decimal SessionCost()
    {
        if (!EndTime.HasValue)
        {
            throw new InvalidOperationException("Cannot calculate cost for an ongoing parking session.");
        }
        TimeSpan totalTime = EndTime.Value - StartTime;
        decimal totalMinutes = (decimal)totalTime.TotalMinutes;

        decimal cost = 0m;
        DateTime currentTime = StartTime;

        while (currentTime < EndTime.Value)
        {
            if (currentTime.Hour >= 8 && currentTime.Hour < 18)
            {
                cost += 0.24m; //Day rate
            }
            else
            {
                cost += 0.1m; //Night rate
            }

            currentTime = currentTime.AddMinutes(1);
        }

        return cost;
    }

}
