namespace ManagementEventsAPI.DTOs;

public class CreateSessionDTO
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? SpeakerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? RoomName { get; set; }
}