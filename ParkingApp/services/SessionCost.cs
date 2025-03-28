using ParkingApp.models;

namespace ParkingApp.services;

public static class SessionCost
{
    public static decimal ParkingSessionCost(ParkingSession session)
    {
        if (session == null)
        {
            throw new ArgumentNullException (nameof (session), "Session cannot be null."); 
        }
        
        if (!session.EndTime.HasValue)
       {
           throw new InvalidOperationException("Cannot calculate cost for an ongoing parking session.");
       }

       DateTime endTime = session.EndTime.Value; 
       TimeSpan totalTime = endTime - session.StartTime;

       decimal currentCost = 0m;
       DateTime currentTime = session.StartTime;

       while (currentTime < endTime)
       {
           if (currentTime.Hour >= 8 && currentTime.Hour < 18)
           {
               currentCost += 0.24m; //Day rate
           }
           else
           {
               currentCost += 0.1m; //Night rate
           }

           currentTime = currentTime.AddMinutes(1);
       }

       return currentCost;

    }
}
