using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using userMangment.ViewModels;


namespace userMangment.Controllers;

public class GymClassController : Controller
{
     private readonly UserManager<ApplicationUser> _userManager;
     private readonly RoleManager<IdentityRole> _rolManager;
     private readonly IGymService _gymService;
     private readonly IBookingService _bookingService;

     protected event EventHandler<GymClass>? OnGymAdded;
     public GymClassController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IGymService gymService, IBookingService bookingService)
     {
        _gymService = gymService;
        _bookingService = bookingService;
        _rolManager = roleManager;
        _userManager = userManager;

            //this.OnGymAdded+= MyReactionToAddedGym;
    }

    private void MyReactionToAddedGym(object? sender , GymClass gym)
    {
        TempData["EventAlert"]= ($"A new gym class '{gym.Name}' was created!");
    }




    // GET: GymClass
    [HttpGet]
    [ResponseCache(NoStore =true, Location =ResponseCacheLocation.None)]
    public async Task<IActionResult> Create()
    {
        var allClasses =  await _gymService.GetAllClassesAsync();
        var currentUser = await _userManager.GetUserAsync(User);

        var userId = currentUser?.Id;
         if(allClasses == null)
        {
            return NotFound();
        }

        var viewModel = allClasses.Where(c=> c.EndTime > DateTime.UtcNow).Select(g => new GymClassViewModel
        {
            Id = g.Id,
            ClasName = g.Name ?? "Unknown Class",
            Description = g.Desctiption ?? "No description",
            StartTime = g.StartTime,
            Duration = g.Duration,
            ISBokingByUser = userId != null && User.Identity.IsAuthenticated && g.AttendingMembers.Any(u => u.Id == userId)
        });  

       
        return View(viewModel);
    }


    [HttpGet]
    public IActionResult CreateGymClass(string? returnUrl = null)
    {
        
        //ViewBag.ReturnUrl = returnUrl ?? Url.Action("AdminDashboard", "Admin");
        var now = DateTime.Now;
        
        var roundedTime = new DateTime(now.Year, now.Month,now.Day, now.Hour,0, 0);

        var viewModel = new GymClassFormViewModel
        {
            StartTime = roundedTime
        };

       
        return View(viewModel);
    }


    [HttpPost]
     public async Task<IActionResult> CreateGymClass(GymClassFormViewModel model, string? returnUrl = null)
    {
        if(!ModelState.IsValid)
        {
             //ViewBag.ReturnUrl = returnUrl ?? Url.Action("AdminDashboard", "Admin");
            return View(model);
        }

        if(model.StartTime <= DateTime.Now)
        {
            ModelState.AddModelError("StartTime", "Start time must be in the future");
            //ViewBag.ReturnUrl = returnUrl ?? Url.Action("AdminDashboard", "Admin");
            return View(model);
        }

        var gymClass = new GymClass
        {
          Id = model.Id,
          Name = model.ClassName,
          Desctiption = model.Description,
          Duration = model.Duration,
          StartTime = model.StartTime,
           
        };

        var result = await _gymService.CreateAsync(gymClass);

        if(result)
        {
            TempData["Success"] = "The class was successfully created";
        }
        else
        {
            TempData["Error"] = "Sorry, the class was not created. There was an unexpected error.";
            return View(model);
        }


        //if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) )
        //{
        //    return Redirect(returnUrl);
        //}
        //ViewBag.ReturnUrl = returnUrl ?? Url.Action("AdminDashboard", "Admin");

        return RedirectToAction("Create", new {returnUrl});
        
    }
        
    [HttpGet]
    public async Task<IActionResult> EditGymClass(int id)
    {
        if (id == 0) 
        {
            return NotFound();
        }
        var gymClass =await _gymService.GetByIdAsync(id);

        if (gymClass == null)
        {
            return NotFound();
        }
        
        var model = new GymClassFormViewModel
        {
            
          Id = gymClass.Id,
          ClassName = gymClass.Name ?? string.Empty,
          StartTime = gymClass.StartTime,
          Duration = gymClass.Duration,
          Description = gymClass.Desctiption  
        };

        return View(model);
    }

   [HttpPost]
   public async Task<IActionResult> EditGymClass(GymClassFormViewModel model, string returnUrl)
    {

    if(!ModelState.IsValid)
    {          
        return View(model);
    }

    if(model.StartTime <= DateTime.UtcNow)
    {
            ModelState.AddModelError("StartTime", "Start time must be in the future");
             ViewBag.ReturnUrl =  returnUrl ?? Url.Action("AdminDashboard", "Admin"); 
            return View(model);
    }

    var gymClass = new GymClass
    {
        Id = model.Id,
        Name = model.ClassName,
        StartTime = model.StartTime,
        Duration = model.Duration,
        Desctiption = model.Description
    };
   

    var result= await _gymService.UpdateAsync(gymClass);

    if(result)
    {
        TempData["Success"] = "The class was successfully updated";
    }
    else
    {
        TempData["Error"] = "Sorry, the class was not updated. There was an unexpected error.";
        return View(model);
    }


    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("AdminDashboard", "Admin");

    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {

        GymClass? gymClass = await _gymService.GetByIdAsync(id);

        if(gymClass ==null)
        {
            return NotFound();
        }

        var member = gymClass.AttendingMembers.Select(i=> i.UserName).ToList();

         return Json(member);
            
        }

        
        public async Task<IActionResult> DeleteClass(GymClassViewModel delete)
        {

        if(!ModelState.IsValid)
        {
            return View(delete);
        }

        if (delete.Id <= 0)
        {
            ModelState.AddModelError("Delete", "there is no class to delete");
            return View("Create");
        }


        var result =  await _gymService.DeleteAsync(delete.Id);

        if(result)
        {
            TempData["Success"] = "The class deleted successfully";
        }
        else
        {
            TempData["Error"] = "Sorry, the class can't removed. There is an unexpected error.";
        }

        // if(string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        // {
        //     return Redirect(returnUrl);
        // }

            return RedirectToAction("Create");

    }



     [HttpPost]
     [ValidateAntiForgeryToken]
     [Authorize]
     [ResponseCache(NoStore =true, Location=ResponseCacheLocation.None)]
    public async Task<IActionResult> ToggleBooking(int id)
    {
        var userId =  _userManager.GetUserId(User);
        if(userId == null)
        {
            return Challenge();
        }

        var success= await _bookingService.ToggleBookingAsync(id , userId ?? "N");

        if(!success)
        {
            TempData["Error"] = "The reservation cannot be changed. Please confirm the time and availability of the class.";
        }
        else
        {
            TempData["Success"] = "Your booking status has been successfully updated!";
        }


        return RedirectToAction("Create");
    }


   
    
}


