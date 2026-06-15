using ManagementEventsAPI.DTOs;
using ManagementEventsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagementEventsAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    // GET - api/user/{userId}/schedule
    [HttpGet("{userId}/schedule")]
    public ActionResult GetUserSchedule(int userId)
    {
        try
        {
            List<UserScheduleDTO>? userSchedule = _userService.GetUserSchedule(userId);
            if (userSchedule == null)
            {
                return NotFound("User not found");
            }
            return Ok(userSchedule);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // For MySchedule - getting user's details
    // GET - api/user/{userId}
    [HttpGet("{userId}")]
    public ActionResult GetUserDetails(int userId)
    {
        try
        {
            UserDetailsDTO? user = _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}