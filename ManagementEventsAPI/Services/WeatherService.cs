using System.Text.Json;
using ManagementEventsAPI.DTOs;
using Data.Models;
using Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace ManagementEventsAPI.Services;

public class WeatherService
{
    private readonly EventRepository _eventRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;

    public WeatherService(
        EventRepository eventRepository,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _eventRepository = eventRepository;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    // Tenth row in the table - /api/event/{id}/weather GET
    public async Task<WeatherDTO?> GetWeatherForEvent(int eventId)
    {
        Event? eventFromDB = _eventRepository.GetEventById(eventId);
        if (eventFromDB == null)
        {
            return null;
        }
        string eventDate = eventFromDB.StartDate.ToString("yyyy-MM-dd");
        string cacheKey = "weather_event_" + eventId + "_" + eventDate;
        
        if (_cache.TryGetValue(cacheKey, out WeatherDTO? cachedWeather))
        {
            return cachedWeather;
        }
        var client = _httpClientFactory.CreateClient();

        string apiKey = "f1854177792046ffaa973219261206";
        string location = eventFromDB.Location;
        int eventHour = eventFromDB.StartDate.Hour;

        string weatherUrl =
            "https://api.weatherapi.com/v1/future.json?key=" + apiKey +
            "&q=" + Uri.EscapeDataString(location) + "&dt=" + eventDate;

        string weatherJsonString = await client.GetStringAsync(weatherUrl);
        JsonDocument weatherJson = JsonDocument.Parse(weatherJsonString);

        JsonElement forecastDay = weatherJson.RootElement.GetProperty("forecast").GetProperty("forecastday")[0];

        JsonElement hourForecast = forecastDay.GetProperty("hour")[eventHour];

        string temperature = hourForecast.GetProperty("temp_c").GetDouble().ToString();
        string feelsLike = hourForecast.GetProperty("feelslike_c").GetDouble().ToString();
        string humidity = hourForecast.GetProperty("humidity").GetInt32().ToString();
        string weatherDescription = hourForecast
            .GetProperty("condition")
            .GetProperty("text")
            .GetString()!;
        
        WeatherDTO weather = new WeatherDTO
        {
            EventId = eventFromDB.Id,
            EventTitle = eventFromDB.Title,
            Location = eventFromDB.Location,
            EventDate = eventFromDB.StartDate,
            WeatherInfo =
                "Weather: " + weatherDescription + ", " +
                "Temperature: " + temperature + "°C, " +
                "Feels like: " + feelsLike + "°C, " +
                "Humidity: " + humidity + "%"
        };

        _cache.Set(cacheKey, weather, TimeSpan.FromMinutes(10));
        return weather;
    }
}