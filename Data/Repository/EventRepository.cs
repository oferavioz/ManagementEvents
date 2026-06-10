using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository;

public class EventRepository
{
    private readonly EventSystemContext _context;

    public EventRepository(EventSystemContext context)
    {
        _context = context;
    }

    public void AddEvent(Event newEvent)
    {
        _context.Events.Add(newEvent);
    }

    public Event? GetEventById(int id)
    {
        return _context.Events
            .FirstOrDefault(e => e.Id == id);
    }

    public Event? GetEventWithSessions(int id)
    {
        return _context.Events
            .Include(e => e.Sessions)
            .FirstOrDefault(e => e.Id == id);
    }

    public Event? GetEventWithSessionsAndRegistrations(int id)
    {
        return _context.Events
            .Include(e => e.Sessions)
            .ThenInclude(s => s.SessionRegistrations)
            .FirstOrDefault(e => e.Id == id);
    }

    public List<Event> GetEventSchedule()
    {
        return _context.Events
            .Include(e => e.Sessions)
            .OrderBy(e => e.StartDate)
            .ToList();
    }

    public void RemoveRegistrations(IEnumerable<SessionRegistration> registrations)
    {
        _context.SessionRegistrations.RemoveRange(registrations);
    }

    public void RemoveSessions(IEnumerable<Session> sessions)
    {
        _context.Sessions.RemoveRange(sessions);
    }

    public void RemoveEvent(Event eventToDelete)
    {
        _context.Events.Remove(eventToDelete);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}