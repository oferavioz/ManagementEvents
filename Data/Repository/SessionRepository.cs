using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository;

public class SessionRepository
{
    private readonly EventSystemContext _context;

    public SessionRepository(EventSystemContext context)
    {
        _context = context;
    }

    public Event? GetEventById(int eventId)
    {
        return _context.Events
            .FirstOrDefault(e => e.Id == eventId);
    }

    public Session? GetSessionById(int sessionId)
    {
        return _context.Sessions
            .FirstOrDefault(s => s.Id == sessionId);
    }

    public User? GetUserById(int userId)
    {
        return _context.Users
            .FirstOrDefault(u => u.Id == userId);
    }

    public bool IsUserAlreadyRegistered(int sessionId, int userId)
    {
        return _context.SessionRegistrations
            .Any(sr => sr.SessionId == sessionId && sr.UserId == userId);
    }

    public List<SessionRegistration> GetUserRegistrationsWithSessions(int userId)
    {
        return _context.SessionRegistrations
            .Include(r => r.Session)
            .Where(r => r.UserId == userId)
            .ToList();
    }

    public List<SessionRegistration> GetRegistrationsBySessionIdWithUsers(int sessionId)
    {
        return _context.SessionRegistrations
            .Include(sr => sr.User)
            .Where(sr => sr.SessionId == sessionId)
            .ToList();
    }

    public void AddSession(Session newSession)
    {
        _context.Sessions.Add(newSession);
    }

    public void AddRegistration(SessionRegistration newRegistration)
    {
        _context.SessionRegistrations.Add(newRegistration);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}