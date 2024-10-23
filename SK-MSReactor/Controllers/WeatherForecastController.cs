using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace SK_MSReactor.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get(Kernel kernel)
    {
        var TemperatureC = Random.Shared.Next(-20, 55);
        var result = new WeatherForecast{
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = TemperatureC,
            Summary = await kernel.InvokePromptAsync<string>($"Say something about this temperature {TemperatureC} c")
        };
        return Ok(result);
    }
    [HttpPost(Name = "Ask")]
    public async Task<IActionResult> Post(Kernel kernel, string message)
    {
        
        var result = await kernel.InvokePromptAsync<string>(message);
      
        return Ok(result);
    }
}
