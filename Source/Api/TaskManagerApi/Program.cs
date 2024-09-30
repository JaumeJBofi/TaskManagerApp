using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text.Json;
using TaskManagerApi.Middleware;
using TaskManagerApi.Repositories;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Utilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;
using TaskManagerApi.Handlers;



var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);    

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;    // Use camelCase for deserialization
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;                   // Allow case-insensitive matching
    });

    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c=> {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter a valid token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer" // Must be "Bearer"
    });

    // Add a security requirement for the JWT
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Must match the name of the security definition above
                }
            },
            new string[] {}
        }
    });
});
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
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Important for HTTPS
    options.Cookie.SameSite = SameSiteMode.None;    
    
});

var corsPolicy = "AllowFrontendLocalhost"; // You can name the policy anything you want
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4220")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Logging.AddConsole();

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = !isDev; 
    x.SaveToken = true;
    x.UseSecurityTokenValidators = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(config.JwtSettings.JwtKey),
        ValidateIssuer = !isDev,    
        ValidateAudience = !isDev,  
        ValidIssuer = config.JwtSettings.ValidIssuer,
        ValidAudience = config.JwtSettings.ValidAudience,
        ValidateLifetime = true,
        LogValidationExceptions = true                
    };
});

builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();

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
app.UseCors(corsPolicy);

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseMiddleware<RefreshTokenMiddleware>();

// Enable Authentication and Authorization

app.UseAuthentication();
app.UseAuthorization();


app.UseOpenApi();
app.UseSwaggerUI();


// Map controllers
app.MapControllers(); // This line replaces the previous example endpoint

app.Run();

