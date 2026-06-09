using Microsoft.AspNetCore.Mvc;
using ManagementEventsAPI.Services; 
using ManagementEventsAPI.DTOs;
using Data.Models; 

namespace ManagementEventsAPI.Controllers;

[ApiController]
[Route("api/event")]
public class EventController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly SessionService _sessionService;
    private readonly WeatherService _weatherService;

    public EventController(EventService eventService, SessionService sessionService, WeatherService weatherService)
    {
        _eventService = eventService;
        _sessionService = sessionService;
        _weatherService = weatherService;
    }

    // POST - api/event
    [HttpPost]
    public ActionResult AddEvent([FromBody] CreateEventDTO createEventDTO)
    {
        try
        {
            Event createdEvent = _eventService.AddEvent(createEventDTO);
            return Ok(createdEvent);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET - api/event/{id}
    [HttpGet("{id}")]
    public ActionResult GetEventById(int id)
    {
        try
        {
            EventDetailsDTO? eventFromDB = _eventService.GetEventById(id);
            if (eventFromDB == null)
            {
                return NotFound("Event not found");
            }

            return Ok(eventFromDB);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // PUT - api/event/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateEvent(int id, [FromBody] UpdateEventDTO updateEventDTO)
    {
        try
        {
            Event? updatedEvent = _eventService.UpdateEvent(id, updateEventDTO);
            if (updatedEvent == null)
            {
                return NotFound("Event not found");
            }

            return Ok(updatedEvent);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // DELETE - api/event/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteEvent(int id)
    {
        try
        {
            bool deletedEvent = _eventService.DeleteEvent(id);
            if (!deletedEvent)
            {
                return NotFound("Event not found");
            }

            return Ok("Event deleted successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET - api/event/schedule
    [HttpGet("schedule")]
    public ActionResult GetEventSchedule()
    {
        try
        {
            List<EventDetailsDTO> eventSchedule = _eventService.GetEventSchedule();
            return Ok(eventSchedule);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // POST - /api/event/{eventId}/session
    // This endpoint is implemented here and not in SessionController because it uses /api/event 
    [HttpPost("{eventId}/session")]
    public ActionResult AddSession(int eventId, [FromBody] CreateSessionDTO createSessionDTO)
    {
        try
        {
            SessionDetailsDTO? createdSession = _sessionService.AddSessionToEvent(eventId, createSessionDTO);

            if (createdSession == null)
            {
                return BadRequest("Event not found or session time is outside the event duration");
            }

            return Ok(createdSession);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // WEATHER 
    // GET - api/event/{id}/weather
    // :int - is important so the routing wont confuse it with other routes like schedule
    [HttpGet("{id:int}/weather")]
    public async Task<ActionResult> GetWeatherForEvent(int id)
    {
        try
        {
            WeatherDTO? weather = await _weatherService.GetWeatherForEvent(id);

            if (weather == null)
            {
                return NotFound("Event not found");
            }

            return Ok(weather);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}