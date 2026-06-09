namespace ManagementEventsAPI.DTOs;

public class UpdateEventDTO
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = null!;
    public string EventType { get; set; } = null!;
}