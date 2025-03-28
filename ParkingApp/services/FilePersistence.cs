using System.Text.Json;
using ParkingApp.models;

namespace ParkingApp.services;

public class FilePersistence
{
    private static readonly string UsersFilePath = "users.json";
    private static readonly string ParkingSessionsFilePath = "parkingSessions.json";

    public static List<User> ReadUsers()
    {
        if (!File.Exists(UsersFilePath)) return new List<User>();
        var json = File.ReadAllText(UsersFilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public static void WriteUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UsersFilePath, json);
    }

    public static List<ParkingSession> ReadParkingSessions()
    {
        if (!File.Exists(ParkingSessionsFilePath)) return new List<ParkingSession>();
        var json = File.ReadAllText(ParkingSessionsFilePath);
        return JsonSerializer.Deserialize<List<ParkingSession>>(json) ?? new List<ParkingSession>();
    }

    public static void WriteParkingSessions(List<ParkingSession> parkingSessions)
    {
        var json = JsonSerializer.Serialize(parkingSessions, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ParkingSessionsFilePath, json);
    }
    
}
