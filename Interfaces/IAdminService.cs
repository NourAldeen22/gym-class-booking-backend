using userMangment.Models;
using userMangment.ViewModels;
namespace userMangment.Interfaces;


public interface IAdmminService
{
    Task<List<ApplicationUser>> GetAllUSersListAsync();
    Task<ApplicationUser?> GetUserAsync(string userId);
    Task<int> isUppComingClassAsync();
    Task<ApplicationUser?> AddNewUserAsync(string firstName , string lastName, string email, string password);
    Task<string> MakeMemberAsync(string userId, string currentAdminId);
    Task<List<UserForAdminViewModel>> GetUserForAdminAsync(string currentAdminId);    

    
}