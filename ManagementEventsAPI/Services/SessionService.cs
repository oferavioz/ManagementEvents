using Data.Models;
using Data.Repository;
using ManagementEventsAPI.DTOs;

namespace ManagementEventsAPI.Services;

public class SessionService
{
    private readonly SessionRepository _sessionRepository;

    public SessionService(SessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    // Sixth row in the table - /api/event/{eventId}/session POST
    public SessionDetailsDTO? AddSessionToEvent(int eventId, CreateSessionDTO createSessionDTO)
    {
        Event? eventFromDB = _sessionRepository.GetEventById(eventId);
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

        _sessionRepository.AddSession(newSession);
        _sessionRepository.SaveChanges();

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
        Session? sessionToRegister = _sessionRepository.GetSessionById(sessionId);
        if (sessionToRegister == null)
        {
            return "Session not found";
        }
        // Check if the user exists in the database
        User? userFromDB = _sessionRepository.GetUserById(registerSessionDTO.UserId);
        if (userFromDB == null)
        {
            return "User not found";
        }
        // Check if the user is already registered to this session
        bool alreadyRegistered = _sessionRepository.IsUserAlreadyRegistered(sessionId, registerSessionDTO.UserId);
        if (alreadyRegistered)
        {
            return "User is already registered for this session";
        }
        // Get all registrations for the user
        List<SessionRegistration> userRegistrations =
            _sessionRepository.GetUserRegistrationsWithSessions(registerSessionDTO.UserId);
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

        _sessionRepository.AddRegistration(newRegistration);
        _sessionRepository.SaveChanges();

        return "User registered successfully";
    }

    // Eighth row in the table - /api/session/{sessionId}/user GET 
    public List<UserDetailsDTO>? GetUsersBySessionId(int sessionId)
    {
        Session? sessionFromDB = _sessionRepository.GetSessionById(sessionId);
        if (sessionFromDB == null)
        {
            return null;
        }

        List<UserDetailsDTO> users = _sessionRepository
            .GetRegistrationsBySessionIdWithUsers(sessionId)
            .Select(sr => new UserDetailsDTO
            {
                Id = sr.User.Id,
                FullName = sr.User.FullName,
                Email = sr.User.Email
            }).ToList();
        return users;
    }
}