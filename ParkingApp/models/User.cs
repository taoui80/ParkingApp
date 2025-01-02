using System;

namespace ParkingApp.models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CarLicensePlate { get; set; }
    public decimal AccountDebit { get; set; }
    

    public User() {}

    public User(int id, string name, string carLicensePlate, decimal accountDebit = 0)
    {
        int Id = id;
        string Name = name ?? throw new ArgumentNullException(nameof(name));
        string CarLicensePlate = carLicensePlate ?? throw new ArgumentNullException(nameof(carLicensePlate));
        decimal AccountDebit = accountDebit;
    }      

}
