using System.Net.Http.Json;
using CoffeeMachineApi.Models;
using Microsoft.Extensions.Configuration;
using Polly.Extensions.Http;
using Polly;
using System.Net;
using CoffeeMachineApi.Controllers;
using System.Net.Http;

public class Weather : IWeather
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<Weather> _logger;
    public Weather(HttpClient httpClient, IConfiguration config, ILogger<Weather> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<double?> GetTemperature()
    {
        try
        {
            string apiKey = _config["Weather:ApiKey"]!;
            string city = _config["Weather:City"];
            string urlTemplate = _config["Weather:ApiUrl"]!;
            string url = urlTemplate.Replace("{city}", city).Replace("{apikey}", apiKey);
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);
            return response?.Main?.Temp;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return null;
        }
    }

}
