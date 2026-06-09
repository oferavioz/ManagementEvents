using Data.Data;
using Data.Models;
using ManagementEventsAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ManagementEventsAPI.Services;

public class EventService
{
    private readonly EventSystemContext _context; 
    public EventService(EventSystemContext context)
    {
        _context = context;
    } 
    
    // First row in the table - /api/event POST
    public Event AddEvent(CreateEventDTO createEventDTO)
    {
        var newEvent = new Event
        {
            Title = createEventDTO.Title,
            Description = createEventDTO.Description,
            StartDate = createEventDTO.StartDate,
            EndDate = createEventDTO.EndDate,
            Location = createEventDTO.Location,
            EventType = createEventDTO.EventType
        };
        
        _context.Events.Add(newEvent);
        _context.SaveChanges();
        
        return newEvent;
    }
    
    // Second row in the table - /api/event/{id} GET
    public EventDetailsDTO? GetEventById(int id)
    {
        Event? eventFromDb = _context.Events.Include(e => e.Sessions).FirstOrDefault(e => e.Id == id);
        if (eventFromDb == null)
        {
            return null;
        }
        return ConvertEventToDTO(eventFromDb);
    }
    
    // Third row in the table - /api/event/{id} PUT
    public Event? UpdateEvent(int id, UpdateEventDTO updateEventDTO)
    {
        Event? eventToUpdate = _context.Events.FirstOrDefault(e => e.Id == id);
        if (eventToUpdate == null)
        {
            return null;
        }
        eventToUpdate.Title = updateEventDTO.Title;
        eventToUpdate.Description = updateEventDTO.Description;
        eventToUpdate.StartDate = updateEventDTO.StartDate;
        eventToUpdate.EndDate = updateEventDTO.EndDate;
        eventToUpdate.Location = updateEventDTO.Location;
        eventToUpdate.EventType = updateEventDTO.EventType;
        
        _context.SaveChanges();
        return eventToUpdate;
    }
    
    // Fourth row in the table - /api/event/{id} DELETE
    public bool DeleteEvent(int id)
    {
        Event? eventToDelete = _context.Events.Include(e => e.Sessions)
            .ThenInclude(s => s.SessionRegistrations).FirstOrDefault(e => e.Id == id);
        if (eventToDelete == null)
        {
            return false; // If event not found
        }
        foreach (Session session in eventToDelete.Sessions) // Delete all registrations for each session
        {
            _context.SessionRegistrations.RemoveRange(session.SessionRegistrations);
        }
        _context.Sessions.RemoveRange(eventToDelete.Sessions); // Delete all sessions
        _context.Events.Remove(eventToDelete); // Delete the event
        _context.SaveChanges();
        return true; // If successfully deleted
    }
    
    // Fifth row in the table - /api/event/schedule GET
    public List<EventDetailsDTO> GetEventSchedule()
    {
        List<Event> events = _context.Events.Include(e => e.Sessions).OrderBy(e => e.StartDate).ToList();
        return events.Select(e => ConvertEventToDTO(e)).ToList();
    }
    
    
    // ====== HELPERS =====
    private EventDetailsDTO ConvertEventToDTO(Event eventFromDb)
    {
        return new EventDetailsDTO
        {
            Id = eventFromDb.Id,
            Title = eventFromDb.Title,
            Description = eventFromDb.Description,
            StartDate = eventFromDb.StartDate,
            EndDate = eventFromDb.EndDate,
            Location = eventFromDb.Location,
            EventType = eventFromDb.EventType,

            Sessions = eventFromDb.Sessions.Select(s => new SessionDetailsDTO
            {
                Id = s.Id,
                EventId = eventFromDb.Id,
                Title = s.Title,
                Description = s.Description,
                SpeakerName = s.SpeakerName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                RoomName = s.RoomName
            }).ToList()
        };
    }


}