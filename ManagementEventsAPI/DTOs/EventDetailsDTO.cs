namespace ManagementEventsAPI.DTOs;

public class EventDetailsDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? EventType { get; set; }
    public List<SessionDetailsDTO> Sessions { get; set; } = new List<SessionDetailsDTO>();
}