using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using userMangment.Interfaces;
using userMangment.Models;

namespace userMangment.Services;

public class BookingService : IBookingService
{

    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ILogger<BookingService> _logger;


    public BookingService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<BookingService> logger )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
  
  
    public async Task <bool> ToggleBookingAsync(int id, string userId )
    {
        var gymClass = await _context.GymClasses.Include(i=> i.AttendingMembers).FirstOrDefaultAsync(i=> i.Id == id);

        if(gymClass == null)
        {
            _logger.LogError("class Id not Found: {id}", id);
            return false;
        }

        var user = await _context.Users.FindAsync(userId);

        if(user == null)
        {
            _logger.LogError("User Id not Found: {UserId}", userId);
            return false;
            
        }

        if(gymClass.AttendingMembers.Any(u=> u.Id == userId))
        {
            if(gymClass.IsForbiddenCancle())
            {
                _logger.LogWarning("Cancellations are not permitted less than two hours!");
                return false;
            }
            
            gymClass.AttendingMembers.Remove(user);
            _logger.LogInformation("{UserName}, Booking cancelled successfully: {ClassName}", user.UserName, gymClass.Name);
            
        }   
        else
        {
            if(gymClass.IsBookingClosed())
            {
                _logger.LogWarning("The lesson has already started; you cannot book.");
                return false;
            }

            gymClass.AttendingMembers.Add(user);
            _logger.LogInformation("{UserName}, Booking Confirmed successfully: {ClassName}", user.UserName, gymClass.Name);

        }

        await _context.SaveChangesAsync();

        return true;

    }



    
}