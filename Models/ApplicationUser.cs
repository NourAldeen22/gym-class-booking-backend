using Microsoft.AspNetCore.Identity;

namespace userMangment.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName {get; set;}

    public string? LastName {get; set;}

    public string FullName => $"{FirstName} {LastName}";

    public DateTime TimeOfRegistration {get; set;}
    

    public virtual ICollection<GymClass> AttendedClasses {get; set;} = new List<GymClass>();

    public ApplicationUser()
    {
        TimeOfRegistration = DateTime.UtcNow;
    }




}
