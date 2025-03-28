namespace ParkingApp.models;
public class User
{
    public int UserID { get; set; } = 0;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Licenseplate { get; set; }
    public decimal Balance { get; set; } = 0m;
    
    public User() {}
    public User(int userid, string username, string password, string firstname, string lastname, string licenseplate, decimal balance)
    {
        UserID = userid;
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Firstname = firstname ?? throw new ArgumentNullException();
        Lastname = lastname ?? throw new ArgumentNullException();
        Licenseplate = licenseplate ?? throw new ArgumentNullException(nameof(licenseplate));
        Balance = balance;
    }
}
