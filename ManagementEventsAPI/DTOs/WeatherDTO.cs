namespace ManagementEventsAPI.DTOs;

public class WeatherDTO
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string WeatherInfo { get; set; } = null!;
}