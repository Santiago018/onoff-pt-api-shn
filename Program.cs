using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnOff.Api.Application.Interfaces;
using OnOff.Api.Application.Services;
using System.Text;
using OnOff.Api.Infrastructure.Logging;



var builder = WebApplication.CreateBuilder(args);

//Logger
var logPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "Logs",
    $"onoff-log-{DateTime.Now:yyyy-MM-dd}.txt"
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new FileLoggerProvider(logPath));

// Controllers
builder.Services.AddControllers();

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ONOFF_SUPER_SECRET_KEY_123456_2025!!")
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
