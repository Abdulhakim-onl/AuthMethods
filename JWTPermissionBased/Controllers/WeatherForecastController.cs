using JWTPermissionBased.Application.Common.Attributes;
using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JWTPermissionBased.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IPermissionService _permission;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IPermissionService permission)
    {
        _logger = logger;
        _permission = permission;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Permission(nameof(PermissionEnum.GetWeather))]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        //await _permission.ValidatePermission(nameof(PermissionEnum.GetWeather));

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}