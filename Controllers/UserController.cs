using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using userMangment.ViewModels;


namespace userMangment.Controllers;

public class UserController : Controller
{

    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }


   [HttpGet]
    public async Task<IActionResult> UserProfile()
    {

        var userId = _userManager.GetUserId(User);

        if(string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }
       
       var user = await _userService.USerProfile(userId);

       if(user == null)
        {
            return NotFound();
        }
        
        var viewModel = new UserProfileViewModel
        {
          UserId = user.Id,
          FullName = user.FullName,
          Email = user.Email,
          IsMember = User.IsInRole("Member"),
          Bookings = user.AttendedClasses.OrderBy(i=> i.StartTime).Select(b=> new UserBookingViewModel
          {
           
            BookingId = b.Id,
            ClassName = b.Name,
            ClassTime = b.EndTime
          }).ToList()
        };

       if(user.AttendedClasses == null|| !user.AttendedClasses.Any())
        {
            ModelState.AddModelError("", "NO booking yet");     
        }

        return View(viewModel);
    }

    
      [HttpPost]
       public async Task<IActionResult> CancelBooking (int bookingId)
       {
        var userId = _userManager.GetUserId(User);

        if(string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var result = await _userService.CancelBokingAsync(bookingId, userId);

        switch(result)
        {
            case "Success":
            TempData["Success"] = "Your booking has been cancelled successfully.";
            break;
            
            case "To Late":
            TempData["Error"] = "Sorry, you cannot cancel less than 2 hours before the class!";
            break;

             case "class started":
            TempData["Error"] = "The lesson has already started!";
            break;

            case "Not Registered": 
            TempData["Error"] = "You are not registered in this class.";
            break;

            case "Not Found": 
            TempData["Error"] = "The requested class could not be found.";
            break;

            default:
            TempData["Error"] = "Something went wrong. Please try again.";
            break;
        }


        // 🟢 التعديل السحري هنا:
        // جلب رابط الصفحة التي أرسلت الطلب     
        string returnUrl = Request.Headers["Referer"].ToString();
        // إذا وجد الرابط يعيده إليه، وإذا لم يجده يعيده لصفحة البروفايل كخطة احتياطية      
        if(string.IsNullOrWhiteSpace(returnUrl))
        {
            return Redirect(returnUrl);
        }


        return RedirectToAction("UserProfile");
    }


    
















    
}