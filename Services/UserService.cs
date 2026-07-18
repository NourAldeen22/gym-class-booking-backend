using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using userMangment.ViewModels;
using userMangment.Interfaces;
using userMangment.Models;
using System.Security.Claims;
using System.Security;
using Microsoft.Build.Experimental.ProjectCache;


namespace userMangment.Services;
public class UserServiece : IUserService, IAdmminService
{
    
private readonly ApplicationDbContext _context;
private readonly UserManager<ApplicationUser> _userManager;
private readonly RoleManager<IdentityRole> _roleManager;
private readonly ILogger<UserServiece> _logger;

public UserServiece(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserServiece> logger )
{
    _context = context;
    _userManager = userManager;
    _roleManager =roleManager;
    _logger = logger;        
}


public async Task<ApplicationUser?> GetUserWithClassesAsync(string userId)=> await _userManager.Users.Include(i=> i.AttendedClasses).FirstOrDefaultAsync(i=> i.Id == userId);

public async Task<List<ApplicationUser>> GetAllUSersListAsync() => await _userManager.Users.Include(i=> i.AttendedClasses).ToListAsync();

public async Task<ApplicationUser?> GetUserAsync(string userId)=> await _userManager.Users.FirstOrDefaultAsync(i=> i.Id == userId);

public IQueryable<GymClass> GetAllClassesAsync() =>  _context.GymClasses;


public async Task<ApplicationUser?> USerProfile(string userId)
{
    if(string.IsNullOrWhiteSpace(userId))
    {
        _logger.LogWarning("UserProfile requested with a null or empty userId");
        return null;
    }

    var userWithClasses = await GetUserWithClassesAsync(userId);

    if(userWithClasses == null)
    {
        _logger.LogWarning("User With ID {UserID} was not found in database", userId);
        return null;        
    }
    
    if(userWithClasses.AttendedClasses == null || !userWithClasses.AttendedClasses.Any())
    {
            _logger.LogInformation("User {UserName} has no attended gym classes yet.", userWithClasses.UserName);
    }

    return userWithClasses;
}


public async Task<string> CancelBokingAsync(int bookingId, string userId)
{
    var booking = await _context.GymClasses.Include(i=> i.AttendingMembers).FirstOrDefaultAsync(g=> g.Id == bookingId);

    if(booking == null)
    {
        _logger.LogWarning("No Booking yet ");
        return "Not Found";        
    }


    var user = booking.AttendingMembers.FirstOrDefault(u=> u.Id == userId);

    if(user == null)
    {
        _logger.LogWarning("User {UserId} is not registered in class {BookingId}", userId, bookingId);
        return "Not Registered";
    }


     if(booking.IsBookingClosed())
    {
        _logger.LogWarning("The lesson has already started; you cannot book.");
         return "class started";  
    }


    if(booking.IsForbiddenCancle())
    {
        _logger.LogWarning("Cancellations are not permitted less than two hours!");
        return "To Late";        
    }

    booking.AttendingMembers.Remove(user);
    await _context.SaveChangesAsync();

    return "Success";
         
}

public async Task<ApplicationUser?> AddNewUserAsync(string firstName , string lastName, string email, string password)
{

    var existingUser = await _userManager.FindByEmailAsync(email);
    if(existingUser != null)
    {
        _logger.LogWarning("email is exisist {UserEmail}", email);
         return null;        
    }

    var newUser = new ApplicationUser
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        UserName = email,
        
        TimeOfRegistration = DateTime.UtcNow,
        

    };
    
        var result = await _userManager.CreateAsync(newUser, password);
        if(result.Succeeded)
        {
            _logger.LogInformation("New user Create seccessfully");
           return newUser;      
        }

        foreach(var error in result.Errors)
        {
            _logger.LogError(error.Description);
        }
        

    return null;

}


public async Task<string> MakeMemberAsync(string userId, string currentAdminId)
{

    var currentUser = await GetUserAsync(userId);

    if(currentUser == null)    
    {
        _logger.LogWarning("User with ID {UserId} not found.", userId);
        return "NotFound";        
    }

    if(currentUser.Id == currentAdminId)
    {
        _logger.LogWarning("Admin {AdminID} tried to modify their own roles!",currentAdminId );
        return "SelfModification";
    }

    if(!await _roleManager.RoleExistsAsync("Member"))
    {
       await _roleManager.CreateAsync(new IdentityRole("Member"));        
    }


    if(!await _userManager.IsInRoleAsync(currentUser, "Member"))
    {
        var AddMember = await _userManager.AddToRoleAsync(currentUser, "Member");  

        if(!AddMember.Succeeded)
        {
          _logger.LogError("Failed to assign Member role to user ID: {userId}", userId);
          return "Failed";
        }
        
         _logger.LogInformation("Role Assigned: Member role to {userId}", userId);
          return "Assigned";
            
    }
    else
        {
            var unRole =await _userManager.RemoveFromRoleAsync(currentUser, "Member");
            if(!unRole.Succeeded)
            {
                _logger.LogError("Failed to Remove role from user {UserId}. Error occurred.", userId);
                return "Failed";
            }
           
                _logger.LogInformation("Role removed: Member from {userId}", userId);
                return "Removed";
           
        }  
}


public async Task<List<UserForAdminViewModel>> GetUserForAdminAsync(string currentAdminId)
{
    var users = await GetAllUSersListAsync();
    var viewModelList = new List<UserForAdminViewModel>();


    foreach(var user in users.Where(u=> u.Id != currentAdminId))
    {
        viewModelList.Add(new UserForAdminViewModel
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsMember =  await _userManager.IsInRoleAsync(user, "Member")
        });        
    }

    return viewModelList;        
}


public async Task<int> isUppComingClassAsync()
{
    var allclasses = GetAllClassesAsync();

     return await allclasses.CountAsync(i=> i.StartTime >= DateTime.UtcNow);

}



  
}