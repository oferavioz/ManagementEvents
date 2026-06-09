using ManagementEventsAPI.DTOs;
using ManagementEventsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagementEventsAPI.Controllers;

[ApiController]
[Route("api/session")]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;
    
    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    // POST - api/session/{sessionId}/register
    [HttpPost("{sessionId}/register")]
    public ActionResult RegisterUserToSession(int sessionId, [FromBody]RegisterSessionDTO registerSessionDTO)
    {
        try
        {
            string result = _sessionService.RegisterUserToSession(sessionId, registerSessionDTO);
            if (result != "User registered successfully")
            {
                return BadRequest(result);
            }
            return Ok("User registered successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // GET - api/session/{sessionId}/user
    [HttpGet("{sessionId}/user")]
    public ActionResult GetUsersBySessionId(int sessionId)
    {
        try
        {
            List<UserDetailsDTO>? users = _sessionService.GetUsersBySessionId(sessionId);
            if (users == null)
            {
                return NotFound("Session not found");
            }
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    
}