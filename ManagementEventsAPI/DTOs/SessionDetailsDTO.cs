namespace ManagementEventsAPI.DTOs;

public class SessionDetailsDTO
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? SpeakerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? RoomName { get; set; }
}