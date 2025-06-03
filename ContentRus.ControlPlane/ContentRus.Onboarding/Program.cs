var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddHostedService<RabbitMQProvisioningConsumer>();

var app = builder.Build();

app.UseHttpsRedirection();


app.Run();
