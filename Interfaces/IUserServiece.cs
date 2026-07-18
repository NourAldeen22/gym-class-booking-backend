using userMangment.Models;

namespace userMangment.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserAsync(string userId);
    Task<ApplicationUser?> USerProfile(string userId);
    Task<ApplicationUser?> GetUserWithClassesAsync(string userId);
     Task<string> CancelBokingAsync(int bookingId, string userId);

}