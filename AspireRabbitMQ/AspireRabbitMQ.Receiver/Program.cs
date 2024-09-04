var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRabbitMQClient("messaging");

// Add services to the container.
builder.Services.AddHostedService<ProcessRabbitMQMessage>();

var app = builder.Build();

app.Run();