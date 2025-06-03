using ContentRus.TenantManagement.Models;
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
using ContentRus.TenantManagement.RabbitMQ;

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

builder.Services.Configure<RabbitMqSettings>(options =>
{
    options.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    options.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    options.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
    options.QueueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE") ?? "event_queue";
});



builder.Services.AddHostedService<RabbitMqConsumerService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInterfaceRequests",
        policy =>
        {
            policy.WithOrigins("http://selfprovision")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.TenantPlans.Any())
    {
        var basicPriceId = Environment.GetEnvironmentVariable("PRICE_ID_BASIC");
        var proPriceId = Environment.GetEnvironmentVariable("PRICE_ID_PRO");
        var enterprisePriceId = Environment.GetEnvironmentVariable("PRICE_ID_ENTERPRISE");

        dbContext.TenantPlans.AddRange(
            new TenantPlan
            {
                Id = TenantTier.Basic,
                Name = "Basic",
                Price = 9.99,
                PriceId = basicPriceId,
                Features = new List<string>
                {
                    "Create Websites",
                    "Tenant Management",
                }
            },
            new TenantPlan
            {
                Id = TenantTier.Pro,
                Name = "Pro",
                Price = 40,
                PriceId = proPriceId,
                Features = new List<string>
                {
                    "Create Websites",
                    "Tenant Management",
                    "More storage space",
                }
            },
            new TenantPlan
            {
                Id = TenantTier.Enterprise,
                Name = "Enterprise",
                Price = 60,
                PriceId = enterprisePriceId,
                Features = new List<string>
                {
                    "Create Websites",
                    "Tenant Management",
                    "More storage space",
                    "Object storage in cloud",
                }
            }
        );

        dbContext.SaveChanges();
    }
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
