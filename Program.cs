using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using shipping_api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    var uri = new Uri(databaseUrl!);
    var userInfo = uri.UserInfo.Split(':');
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]}";
    options.UseNpgsql(connectionString);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SwiftCargoSecretKey123456789012")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins("http://localhost:5173", "https://shipping-dashboard-three-navy.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();