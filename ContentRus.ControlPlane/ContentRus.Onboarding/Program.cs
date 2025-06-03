using Microsoft.Extensions.Options;
using ContentRus.Onboarding.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.Configure<RabbitMqSettings>(options =>
{
    options.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    options.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    options.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
    options.QueueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE") ?? "event_queue";
});

builder.Services.AddHostedService<RabbitMQProvisioningConsumer>();
builder.Services.AddSingleton<RabbitMQTenantStatusPublisher>();

var app = builder.Build();

app.UseHttpsRedirection();


app.Run();
