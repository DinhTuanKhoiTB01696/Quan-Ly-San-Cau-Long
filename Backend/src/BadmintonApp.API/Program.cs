using BadmintonApp.Application.Interfaces;
using BadmintonApp.Infrastructure.Data;
using BadmintonApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Background Services
builder.Services.AddHostedService<BadmintonApp.API.BackgroundServices.MatchExpirationService>();

// --- Database ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Dependency Injection ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourtService, CourtService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();

// --- JWT Authentication ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    try
    {
        context.Database.EnsureCreated();
        
        // Add IsLocked column if not exists
        context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'IsLocked' AND Object_ID = Object_ID(N'Users')) BEGIN ALTER TABLE Users ADD IsLocked bit NOT NULL DEFAULT 0; END");

        // Add Credits column if not exists
        context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Credits' AND Object_ID = Object_ID(N'Users')) BEGIN ALTER TABLE Users ADD Credits int NOT NULL DEFAULT 3; END");

        // Create Transactions table if not exists
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' and xtype='U')
            CREATE TABLE Transactions (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                UserId INT NOT NULL,
                Amount DECIMAL(18,2) NOT NULL,
                CreditsAdded INT NOT NULL,
                Status INT NOT NULL DEFAULT 0,
                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
            );");

        await BadmintonApp.Infrastructure.Data.DbSeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

