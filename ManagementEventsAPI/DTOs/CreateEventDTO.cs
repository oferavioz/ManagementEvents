namespace ManagementEventsAPI.DTOs;

public class CreateEventDTO
{
    // when creating an event, we need to provide this information : 
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? EventType { get; set; }
}