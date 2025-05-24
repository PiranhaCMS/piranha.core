using ContentRus.TenantManagement.Services;
using ContentRus.TenantManagement.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using DotNetEnv;
using ContentRus.TenantManagement.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Configuration["JwtSettings:SecretKey"] = jwtKey;
    builder.Services.Configure<JwtSettings>(
        builder.Configuration.GetSection("JwtSettings")
    );
}
else
{
    throw new Exception("JWT_SECRET_KEY is missing from .env");
}

var connectionString = $"Server={Environment.GetEnvironmentVariable("HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DATABASE")};" +
                       $"User={Environment.GetEnvironmentVariable("USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("PASSWORD")};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TenantService>();


// JWT Authentication
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInterfaceRequests",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowInterfaceRequests");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
