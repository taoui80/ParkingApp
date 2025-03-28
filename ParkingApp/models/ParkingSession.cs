namespace ParkingApp.models;
public class ParkingSession
{
    public int PeriodId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime? EndTime { get; set; } 
    public decimal PeriodCost { get; set;} = 0m;
    
    public bool IsActive => EndTime == null; 
    public ParkingSession() {}
    public ParkingSession(int periodId, int userid, DateTime startTime, DateTime? endTime, decimal periodCost)
    {
        PeriodId = periodId;
        UserId = userid;
        StartTime = startTime; 
        EndTime = endTime;
        PeriodCost =  periodCost;   
    }   
}
