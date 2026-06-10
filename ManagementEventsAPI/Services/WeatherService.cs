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

        string cacheKey = "weather_event_" + eventId;
        if (_cache.TryGetValue(cacheKey, out WeatherDTO? cachedWeather))
        {
            return cachedWeather;
        }
        var client = _httpClientFactory.CreateClient();
        string location = eventFromDB.Location;
        string weatherUrl =
            "https://wttr.in/~" + Uri.EscapeDataString(location) + "?format=j1";

        string weatherJsonString = await client.GetStringAsync(weatherUrl);
        JsonDocument weatherJson = JsonDocument.Parse(weatherJsonString);
        JsonElement currentCondition = weatherJson.RootElement
            .GetProperty("current_condition")[0];

        string temperature = currentCondition.GetProperty("temp_C").GetString()!;
        string feelsLike = currentCondition.GetProperty("FeelsLikeC").GetString()!;
        string humidity = currentCondition.GetProperty("humidity").GetString()!;
        string weatherDescription = currentCondition.GetProperty("weatherDesc")[0]
            .GetProperty("value").GetString()!;

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