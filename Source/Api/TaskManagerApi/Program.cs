using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text.Json;
using TaskManagerApi.Middleware;
using TaskManagerApi.Repositories;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);    

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;    // Use camelCase for deserialization
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;                   // Allow case-insensitive matching
    }); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfig, Config>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var isDev = builder.Environment.IsDevelopment();

var config = new Config(builder.Configuration);

var corsPolicy = "AllowFrontendLocalhost"; // You can name the policy anything you want
if (builder.Environment.IsDevelopment())
{     
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicy,
            policy =>
            {

                //policy.WithOrigins("http://localhost:4200")
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()   // Allow all headers (you can be more restrictive if needed)
                      .AllowAnyMethod();  // Allow all HTTP methods (GET, POST, etc.)                      
            });
    });
}

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
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();

builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDev)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RefreshTokenMiddleware>();

// Enable Authentication and Authorization

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.UseOpenApi();
app.UseSwaggerUI();



// Map controllers
app.MapControllers(); // This line replaces the previous example endpoint

app.Run();
