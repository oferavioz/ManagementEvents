using Data.Models;
using Data.Repository;
using ManagementEventsAPI.DTOs;

namespace ManagementEventsAPI.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Ninth row in the table - /api/user/{userId}/schedule GET
    public List<UserScheduleDTO>? GetUserSchedule(int userId)
    {
        User? userFromDB = _userRepository.GetUserById(userId);
        if (userFromDB == null)
        {
            return null;
        }
        // Get all sessions user is registered to
        List<UserScheduleDTO> userSchedule = _userRepository
            .GetUserRegistrationsWithSessions(userId)
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
            })
            .ToList();
        return userSchedule;
    }
    
    // For MySchedule - getting user's details
    public UserDetailsDTO? GetUserById(int userId)
    {
        User? userFromDB = _userRepository.GetUserById(userId);
        if (userFromDB == null)
        {
            return null;
        }
        return new UserDetailsDTO
        {
            Id = userFromDB.Id,
            FullName = userFromDB.FullName,
            Email = userFromDB.Email
        };
    }
}