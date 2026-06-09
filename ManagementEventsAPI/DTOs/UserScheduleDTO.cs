namespace ManagementEventsAPI.DTOs;

// for user schedule page - added the registration date
public class UserScheduleDTO
{
    public int SessionId { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? SpeakerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? RoomName { get; set; }
    public DateTime? RegistrationDate { get; set; }
}