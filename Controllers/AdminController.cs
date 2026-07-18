using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using userMangment.ViewModels;


namespace userMangment.Controllers;

public class AdminController : Controller
{

private readonly IAdmminService _adminService;
private readonly UserManager<ApplicationUser> _userManager;


public AdminController(IAdmminService admminService, UserManager<ApplicationUser> userManager)
{
    _adminService = admminService;
    _userManager = userManager;
}


[HttpGet]

public async Task<IActionResult> AdminDashboard()
{
   var allUsers =  await _adminService.GetAllUSersListAsync();

   if(allUsers == null || !allUsers.Any())
    {
        ModelState.AddModelError(string.Empty, "No user yet");
        allUsers = new List<ApplicationUser>();   
    }

    var currentAdmin = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrWhiteSpace(currentAdmin))
    {
        return Challenge(); // أو RedirectToAction("Login", "Account")
    }

   
   
    var userViewModels = await _adminService.GetUserForAdminAsync(currentAdmin);

    var viewModel = new AdminDashboardViewModel
    {
        TotalUsersInSystem = allUsers.Count(),
        UpcomingClassesCount = await _adminService.isUppComingClassAsync(),
        AllUsers = userViewModels

    };

    return View(viewModel);

}

[HttpPost]
public async Task<IActionResult> MakeMember(string userId)
{
    var currentAdmin = await _userManager.GetUserAsync(User);
    if(currentAdmin == null)
    {
        return Challenge();
    }

    var isSucceeded = await _adminService.MakeMemberAsync(userId, currentAdmin.Id);

    switch(isSucceeded)
    {
        case "Assigned":
            TempData["SuccessMessage"] = "User role assigned successfully!";
        break;
        case "Removed":
            TempData["SuccessMessage"] = "User role removed successfully!";
        break;
        case "SelfModification":
            TempData["Error"] = "You can't modify your own roles!";
        break;
        default:
            TempData["Error"] = "Operation failed. User not found or server error.";
        break;     
    }

   return RedirectToAction(nameof(AdminDashboard)); 
      
}


[HttpPost]
public async Task<IActionResult> AddNewUser(AddUserViewModel model)
{
   if(!ModelState.IsValid)
    {
     var errorMessages = ModelState.Where(x=> x.Value?.Errors.Count> 0).Select(x=> $"{x.Key} : {string.Join(", ", x.Value.Errors.Select(e=> e.ErrorMessage))}");

     TempData["ErrorMessage"] = "Error create cause:  " + string.Join(" | ", errorMessages);
     return RedirectToAction(nameof(AdminDashboard)); 
     
    }

    var resultuser = await _adminService.AddNewUserAsync(
        firstName: model.FirstName, 
        lastName:model.LastName , 
        email:model.Email, 
        password:model.Password
       
        
        
    );

    if(resultuser == null)
    {
        ModelState.AddModelError(string.Empty, "This email address is already registered in the system."); 
        return RedirectToAction(nameof(AdminDashboard));    
    }

    TempData["SuccessMessage"] = "New user created successfully!";

    return RedirectToAction(nameof(AdminDashboard));
}





    
}