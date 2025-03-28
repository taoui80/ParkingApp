using ParkingApp.models;
using ParkingApp.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
//app.UseAuthorization();

var users = FilePersistence.ReadUsers();
var parkingSessions = FilePersistence.ReadParkingSessions();

int userCounter = users.Any() ? users.Max(u => u.UserID) + 1 : 1;
int sessionCounter = parkingSessions.Any() ? parkingSessions.Max(s => s.PeriodId) + 1 : 1;


// Register user
app.MapPost("/register-user", (User user) =>
{
    if (users.Any(u => u.Username == user.Username))
    {
        return Results.BadRequest("Username already exists.");
    }

    user.UserID = userCounter++;
    users.Add(user);
    FilePersistence.WriteUsers(users); 

    return Results.Ok(new { message = "Successfully registered!", user }); 
});

// Login user
app.MapPost("/login", (User loginRequest) =>
{
    var users = FilePersistence.ReadUsers(); 
    var user = users.FirstOrDefault(u => u.Username == loginRequest.Username);    

    if (user == null || user.Password != loginRequest.Password) 
    {
        return Results.BadRequest("Invalid credentials");
    }
    return Results.Ok(new {
        user = new { 
        userID = user.UserID, 
        firstname = user.Firstname, 
        lastname = user.Lastname, 
        licenseplate = user.Licenseplate, 
        balance = user.Balance 
        }
    });   
});

app.MapGet("/previous-sessions/{userID}", (int userID) =>
{
    var previousSessions = parkingSessions
        .Where(s => s.UserId == userID)
        .OrderByDescending(s => s.StartTime)
        .ToList();

    return Results.Ok(new { previousSession = previousSessions });
});

// Start a parking session
app.MapPost("/start-session", (HttpRequest request) =>
{
    if (!request.Query.TryGetValue("userID", out var userIDString) || !int.TryParse(userIDString, out int userID)) 
    {
        return Results.BadRequest("UserID is required and must be a valid integer");
    }
    
    var user = users.FirstOrDefault(u => u.UserID == userID);
    if (user == null) return Results.NotFound("User not found!");

    var session = new ParkingSession
    {
        PeriodId = sessionCounter++, 
        UserId = user.UserID, 
        StartTime = DateTime.Now, 
        EndTime = null, 
        PeriodCost = 0
    };

    parkingSessions.Add(session);
    FilePersistence.WriteParkingSessions(parkingSessions);

    return Results.Ok(session); 
});

//End a parking session
app.MapPost("/end-session/{userID}", (int userID) =>
{
    var user = users.FirstOrDefault(u => u.UserID == userID);
    if (user == null) return Results.BadRequest("Invalid user ID");

    var session = parkingSessions
        .Where(s => s.UserId == userID && s.EndTime == null)
        .OrderByDescending(s => s.StartTime)
        .FirstOrDefault();

    if (session == null)
    {
        return Results.NotFound("No active session found for this user.");
    }

    session.EndTime = DateTime.Now;
    session.PeriodCost = SessionCost.ParkingSessionCost(session); 
    user.Balance += session.PeriodCost;

    FilePersistence.WriteParkingSessions(parkingSessions);
    FilePersistence.WriteUsers(users);

    return Results.Ok(new { message = "Session ended successfully.", sessionData = session, balance = user.Balance });     
});

//Get current parking session for a user 
app.MapGet("/current-session/{userID}", (int userID) =>
{
    var session = parkingSessions
        .Where(s => s.UserId == userID && s.EndTime == null)
        .OrderByDescending(s => s.StartTime)
        .FirstOrDefault();

    if (session == null)
    {
        return Results.Ok(new { message = "No active session", startTime = "", cost = 0, isActive = false });
    }

    return Results.Ok(session);
});

//Get cost on registered account
app.MapGet("/user-balance/{userID}", (int userID) =>
{
    var user = users.FirstOrDefault(u => u.UserID == userID);
    if (user == null) return Results.NotFound("User not found.");

    return Results.Ok(new { totalBalance = user.Balance });
});

//Get user’s all registered details 
app.MapGet("/user-details/{userID}", (int userID) =>
{
    var user = users.FirstOrDefault(u => u.UserID == userID);

    return user != null ? Results.Ok(user) : Results.NotFound("User not found.");
});


app.Run();


