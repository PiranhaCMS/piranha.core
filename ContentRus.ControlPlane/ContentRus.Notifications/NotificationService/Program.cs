using Notifications.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<RabbitMqSettings>(options =>
{
    options.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    options.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    options.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
    options.QueueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE") ?? "event_queue";
});

builder.Services.AddHostedService<RabbitMQNotificationConsumerService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
/* if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
 */
app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();

