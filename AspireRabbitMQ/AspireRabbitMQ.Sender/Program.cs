using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRabbitMQClient("messaging");

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter( _ => _
.AddFixedWindowLimiter(policyName:"fixed",options =>{
options.PermitLimit = 2;
options.Window = TimeSpan.FromSeconds(10);
options.QueueProcessingOrder = 
    QueueProcessingOrder.OldestFirst;
options.QueueLimit = 2;    
}));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/sendmessage", async Task<IResult>(IConnection connection, string messageToSend) =>{
    var channel = connection.CreateModel();
    channel.QueueDeclare(
        queue:"testMessage",
        durable:false,
        exclusive:false,
        autoDelete:false,
        arguments:null
    );
    var body = Encoding.UTF8.GetBytes(messageToSend);

    channel.BasicPublish(
        exchange:string.Empty,
        routingKey:"testMessage",  
        mandatory:false,
        basicProperties:null,
        body:body
    );
    return Results.Ok(new {});
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
