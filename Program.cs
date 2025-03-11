using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text;
using OnlineStoreBackend.Services;
using OnlineStoreBackend.Helpers;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Allow your React app
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container
builder.Services.AddControllers();

// Setup JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey is not configured in the application settings.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Add services for the application
builder.Services.AddScoped<AuthService>();

// Register the database context
builder.Services.AddDbContext<OnlineStoreContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
    )
);

// Register ApiKeyMiddleware - no need to manually inject
// It will be added to the middleware pipeline in the next step

var app = builder.Build();

// Enable CORS for the frontend
app.UseCors("AllowLocalhost");

// Use the ApiKeyMiddleware to check for valid API_KEY
app.UseMiddleware<ApiKeyMiddleware>(); // Apply the middleware here

// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to handle requests
app.MapControllers();

// Run the application
app.Run();
