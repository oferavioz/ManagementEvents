using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository;

public class UserRepository
{
    private readonly EventSystemContext _context;

    public UserRepository(EventSystemContext context)
    {
        _context = context;
    }

    public User? GetUserById(int userId)
    {
        return _context.Users
            .FirstOrDefault(u => u.Id == userId);
    }

    public List<SessionRegistration> GetUserRegistrationsWithSessions(int userId)
    {
        return _context.SessionRegistrations
            .Include(sr => sr.Session)
            .Where(sr => sr.UserId == userId)
            .OrderBy(sr => sr.Session.StartTime)
            .ToList();
    }
}