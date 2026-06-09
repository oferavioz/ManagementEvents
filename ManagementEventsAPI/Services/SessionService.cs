using Microsoft.EntityFrameworkCore;
using Data.Data;
using ManagementEventsAPI.DTOs;
using Data.Models;

namespace ManagementEventsAPI.Services;

public class SessionService
{
    private readonly EventSystemContext _context;
    
    public SessionService(EventSystemContext context)
    {
        _context = context;
    }
    
    // Sixth row in the table - /api/event/{eventId}/session POST
    public SessionDetailsDTO? AddSessionToEvent(int eventId, CreateSessionDTO createSessionDTO)
    {
        Event? eventFromDB = _context.Events.FirstOrDefault(e => e.Id == eventId);
        if (eventFromDB == null)
        {
            return null;
        }
        if (createSessionDTO.StartTime < eventFromDB.StartDate || 
            createSessionDTO.EndTime > eventFromDB.EndDate || 
            createSessionDTO.StartTime >= createSessionDTO.EndTime)
        {
            throw new Exception("Session time must be within the event duration");
        }
        Session newSession = new Session
        {
            EventId = eventId,
            Title = createSessionDTO.Title,
            Description = createSessionDTO.Description,
            SpeakerName = createSessionDTO.SpeakerName,
            StartTime = createSessionDTO.StartTime,
            EndTime = createSessionDTO.EndTime,
            RoomName = createSessionDTO.RoomName
        };
        _context.Sessions.Add(newSession);
        _context.SaveChanges();
        
        return new SessionDetailsDTO
        {
            Id = newSession.Id,
            EventId = newSession.EventId,
            Title = newSession.Title,
            Description = newSession.Description,
            SpeakerName = newSession.SpeakerName,
            StartTime = newSession.StartTime,
            EndTime = newSession.EndTime,
            RoomName = newSession.RoomName
        };
    }
    
    // Seventh row in the table - /api/session/{sessionId}/register POST
    public string RegisterUserToSession(int sessionId, RegisterSessionDTO registerSessionDTO)
    {
        // Find the session that the user is trying to register for
        Session? sessionToRegister = _context.Sessions.FirstOrDefault(s => s.Id == sessionId);
        if (sessionToRegister == null)
        {
            return "Session not found";
        }
        // Check if the user exists in the database
        User? userFromDB = _context.Users.FirstOrDefault(u => u.Id == registerSessionDTO.UserId);
        if (userFromDB == null)
        {
            return "User not found";
        }
        // Check if the user is already registered to this session
        bool alreadyRegistered = _context.SessionRegistrations.Any(sr => sr.SessionId == sessionId && sr.UserId == registerSessionDTO.UserId);
        if (alreadyRegistered)
        {
            return "User is already registered for this session";
        }
        // Get all registrations for the user
        List<SessionRegistration> userRegistrations = _context.SessionRegistrations.Include(r => r.Session)
            .Where(r => r.UserId == registerSessionDTO.UserId).ToList();
        // Overlap check
        foreach (SessionRegistration registration in userRegistrations)
        {
            Session existingSession = registration.Session;
            bool overlap = sessionToRegister.StartTime < existingSession.EndTime && 
                           sessionToRegister.EndTime > existingSession.StartTime;
            if (overlap)
            {
                return "User already registered to another session at the same time";
            }
        }
        SessionRegistration newRegistration = new SessionRegistration
        {
            SessionId = sessionId,
            UserId = registerSessionDTO.UserId,
            RegistrationDate = DateTime.Now
        };
        
        _context.SessionRegistrations.Add(newRegistration);
        _context.SaveChanges();
        
        return "User registered successfully";
    }
    
    // Eighth row in the table - /api/session/{sessionId}/user GET 
    public List<UserDetailsDTO>? GetUsersBySessionId(int sessionId)
    {
        Session? sessionFromDB = _context.Sessions.FirstOrDefault(s => s.Id == sessionId);
        if (sessionFromDB == null)
        {
            return null;
        }
        List<UserDetailsDTO> users = _context.SessionRegistrations.Include(sr => sr.User)
            .Where(sr => sr.SessionId == sessionId)
            .Select(sr => new UserDetailsDTO
            {
                Id = sr.User.Id,
                FullName = sr.User.FullName,
                Email = sr.User.Email
            }).ToList();
        return users;
    }
    
}