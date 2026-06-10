using Data.Models;
using Data.Repository;
using ManagementEventsAPI.DTOs;

namespace ManagementEventsAPI.Services;

public class EventService
{
    private readonly EventRepository _eventRepository;

    public EventService(EventRepository eventRepository)
    {
        _eventRepository = eventRepository;
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

        _eventRepository.AddEvent(newEvent);
        _eventRepository.SaveChanges();
        return newEvent;
    }

    // Second row in the table - /api/event/{id} GET
    public EventDetailsDTO? GetEventById(int id)
    {
        Event? eventFromDb = _eventRepository.GetEventWithSessions(id);
        if (eventFromDb == null)
        {
            return null;
        }
        return ConvertEventToDTO(eventFromDb);
    }

    // Third row in the table - /api/event/{id} PUT
    public Event? UpdateEvent(int id, UpdateEventDTO updateEventDTO)
    {
        Event? eventToUpdate = _eventRepository.GetEventById(id);
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

        _eventRepository.SaveChanges();
        return eventToUpdate;
    }

    // Fourth row in the table - /api/event/{id} DELETE
    public bool DeleteEvent(int id)
    {
        Event? eventToDelete = _eventRepository.GetEventWithSessionsAndRegistrations(id);
        if (eventToDelete == null)
        {
            return false; // If event not found
        }
        foreach (Session session in eventToDelete.Sessions) // Delete all registrations for each session
        {
            _eventRepository.RemoveRegistrations(session.SessionRegistrations);
        }

        _eventRepository.RemoveSessions(eventToDelete.Sessions); // Delete all sessions
        _eventRepository.RemoveEvent(eventToDelete); // Delete the event
        _eventRepository.SaveChanges();
        return true; // If successfully deleted
    }

    // Fifth row in the table - /api/event/schedule GET
    public List<EventDetailsDTO> GetEventSchedule()
    {
        List<Event> events = _eventRepository.GetEventSchedule();
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