using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using userMangment.Models;
using userMangment.Interfaces;
using userMangment.ViewModels;

namespace userMangment.Controllers;


public class HomeController : Controller
{
    
     private readonly UserManager<ApplicationUser> _userManager;

     private readonly RoleManager<IdentityRole> _roleManager;

     private readonly ILogger<HomeController> _logger;
     private readonly IUserService _userServiece;


    public HomeController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<HomeController> logger, IUserService userServiece)
    {
        
        _userManager = userManager;
        _roleManager = roleManager;
        _userServiece = userServiece;
        _logger = logger;
    }
   
    public async Task<IActionResult> Index()

    {

        if(!await _roleManager.RoleExistsAsync("Admin"))
        {
           await _roleManager.CreateAsync( new IdentityRole("Admin")); 
        }

        var user = new ApplicationUser
       {
         UserName = "me@test.com",
         Email = "me@test.com",
        //  EmailConfirmed = true
        FirstName = "Nour",
        LastName = "Dano",
        TimeOfRegistration = DateTime.UtcNow
           
       };
        
       string password= "Password123!";

      

        var existingUser = await _userManager.FindByEmailAsync(user.Email);
        if (existingUser == null)
        {
            var result = await _userManager.CreateAsync(user, password);
            if(result.Succeeded)
        {
            existingUser = user; // الآن user تم إنشاؤه
        }
        else
        {
        foreach(var error in result.Errors)
        {
            Console.WriteLine(error.Description);
        }
             return View(); // الخروج إذا فشل الإنشاء
        }
        
        }

        // إضافة المستخدم إلى الدور
        if(!await _userManager.IsInRoleAsync(existingUser, "Admin"))
        {
            await _userManager.AddToRoleAsync(existingUser, "Admin");
        }

        var users = await _userManager.Users.ToListAsync();

        if(users == null || !users.Any() )
        {
            ViewData["Error"] = "NO course Found";
            return View(new List<ApplicationUser>());
        }

       
              
        return View(users);
        
    }

    // [HttpGet]
    // public async Task<IActionResult> MakeMember()
    // {
    //     var users = await _userServiece.GetAllUSersListAsync();

    //    var viewModelList = new List<UserProfileViewModel>();

    //    foreach(var user in users)
    //     {
    //         viewModelList.Add(new UserProfileViewModel
    //         {
    //             UserId = user.Id,
    //             FullName = user.UserName,
    //             Email = user.Email,
    //             IsMember = await _userManager.IsInRoleAsync(user, "Member")
    //         });
    //     }

    //     return View(viewModelList);
    // }
    
    //[HttpPost]
    // public async Task<IActionResult> MakeMember (string userId)
    // {

    //     var currentAdmin = await _userManager.GetUserAsync(User);

    //     if(currentAdmin == null)
    //     {
    //         return Challenge();
    //     }

    //     string isSucceeded = await _userServiece.MakeMemberAsync(userId, currentAdmin.Id);

    //     switch(isSucceeded)
    //     {
    //         case "Assigned":
    //         TempData["Success"] = "User role assigned successfully!";
    //         break;

    //         case "Removed":
    //         TempData["Success"] = "User role removed successfully!";
    //         break;

    //         case "SelfModification":
    //         TempData["Error"] = "You can't modify your own roles!";
    //         break;

    //         default:
    //         TempData["Error"] = "Operation failed. User not found or server error.";
    //         break;

    //     }
       
    //     return RedirectToAction("MakeMember");
       
    // }



    [HttpGet]
    public IActionResult CreatClass()
    {
       
        return View();
    }

    // [HttpPost]
    // public async Task<IActionResult> AllUsers()
    // {
    //     var users = await _userManager.Users.Select(i=> i.Id).ToListAsync();

    //     return View(users);

    // }

    public IActionResult ChangePassword()
    {
        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var user = await _userManager.GetUserAsync(User);

        if(user == null)
        {
            return Challenge();
        }


        if(newPassword != confirmPassword)
        {
            ModelState.AddModelError("", " New password and confirmation do not match." );
            return View();
        }

        var cheakPassowrd = await _userManager.CheckPasswordAsync(user, currentPassword);

        if(!cheakPassowrd)
        {
            ModelState.AddModelError("", "Current password is incorrect");
            return View();
        }

        var changePassword = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if(!changePassword.Succeeded)
        {
            foreach(var error in changePassword.Errors)
            {
                ModelState.AddModelError("", error.Description);

            }

          
              return View(user);
        }

     
      TempData["Success"] = "Password changed successfully!";



        return RedirectToAction("Index");
    }



    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
