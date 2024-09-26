using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TaskManagerApi.Middleware;
using TaskManagerApi.Repositories;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add support for controllers

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfig, Config>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var isDev = builder.Environment.IsDevelopment();

var config = new Config(builder.Configuration);

// Configure Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Important for HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Adjust if needed
});


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = !isDev; 
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(config.JwtSettings.JwtKey),
        ValidateIssuer = !isDev,    
        ValidateAudience = !isDev,  
        ValidIssuer = config.JwtSettings.ValidIssuer,
        ValidAudience = config.JwtSettings.ValidAudience,
    };
});


// Add Authorization policies (example)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddSingleton(sp =>
{    
    var mongoClient = new MongoClient(config.MongoDbConnectionString);
    var database = mongoClient.GetDatabase(config.MongoDbDatabaseName);
    return database;
});

builder.Services.AddSingleton<IUserRepository,UserRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDev)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseMiddleware<RefreshTokenMiddleware>();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers(); // This line replaces the previous example endpoint

app.Run();
