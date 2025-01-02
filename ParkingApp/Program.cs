using ParkingApp.models;
using ParkingApp.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var users = FilePersistence.ReadUsers();
var parkingSessions = FilePersistence.ReadParkingSessions();

int userCounter = users.Any() ? users.Max(u => u.Id) + 1 : 1;
int sessionCounter = parkingSessions.Any() ? parkingSessions.Max(s => s.Id) + 1 : 1;    


//Register users and cars
app.MapPost("/register-user", (User user) =>
{
    user.Id = userCounter++;
    users.Add(user);
    FilePersistence.WriteUsers(users); //Persist data to file
    return Results.Created($"/register-user/{user.Id}", user);
});

//Start a parking session
app.MapPost("/start-parking-session", (string carLicensePlate) =>
{
    var user = users.FirstOrDefault(u => u.CarLicensePlate == carLicensePlate);

    if (user == null) return Results.NotFound("User not found!");

    var session = new ParkingSession(sessionCounter++, carLicensePlate, DateTime.UtcNow);
    parkingSessions.Add(session); 
    FilePersistence.WriteParkingSessions(parkingSessions);

    return Results.Created($"/start-parking-session{session.Id}", session);
    
});

//End a parking session
app.MapPost("/end-parking-session", (string carLicensePlate) =>
{
    var session = parkingSessions.FirstOrDefault(s => s.CarLicensePlate == carLicensePlate);

    if(session == null){
        return Results.NotFound("Not active session found for this car.");
    }
   
    session.EndTime = DateTime.UtcNow;
    var user = users.FirstOrDefault(u => u.CarLicensePlate == carLicensePlate);
    user.AccountDebit += session.SessionCost();
    FilePersistence.WriteParkingSessions(parkingSessions);
    FilePersistence.WriteUsers(users);

    return Results.Ok(new {session, user.AccountDebit});

});

//Get current parking session for a car 
app.MapGet("/current-parking-session/{carLicensePlate}", (string? carLicensePlate) =>
{
    var session = parkingSessions.FirstOrDefault(s => s.CarLicensePlate == carLicensePlate);

    return session != null ? Results.Ok(session) : Results.NotFound("No active session");

});

//Get cost on registered account
app.MapGet("/user-account-debit/{carLicensePlate}", (string carLicensePlate) =>
{
    var user = users.FirstOrDefault(u => u.CarLicensePlate == carLicensePlate);
    
    return user != null ? Results.Ok(new {user.Name, user.AccountDebit}) : Results.NotFound("User not found.");
});

//Get user’s all registered details 
app.MapGet("/user-details/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    
    return user != null ? Results.Ok(user) : Results.NotFound("User not found.");    
});

app.Run();


