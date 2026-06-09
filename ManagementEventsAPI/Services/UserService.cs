using Data.Data;
using ManagementEventsAPI.DTOs;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementEventsAPI.Services;

public class UserService
{
    private readonly EventSystemContext _context;
    public UserService(EventSystemContext context)
    {
        _context = context;
    }
    
    // Ninth row in the table - /api/user/{userId}/schedule GET
    public List<UserScheduleDTO>? GetUserSchedule(int userId)
    {
        User? userFromDB = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (userFromDB == null)
        {
            return null;
        }
        // Get all sessions user is regisstered to
        List<UserScheduleDTO> userSchedule = _context.SessionRegistrations.Include(sr => sr.Session)
            .Where(sr => sr.UserId == userId)
            .Select(sr => new UserScheduleDTO()
            {
                SessionId = sr.Session.Id,
                EventId = sr.Session.EventId,
                Title = sr.Session.Title,
                Description = sr.Session.Description,
                SpeakerName = sr.Session.SpeakerName,
                StartTime = sr.Session.StartTime,
                EndTime = sr.Session.EndTime,
                RoomName = sr.Session.RoomName,
                RegistrationDate = sr.RegistrationDate
            }).OrderBy(s => s.StartTime).ToList();
        return userSchedule;
    }

}