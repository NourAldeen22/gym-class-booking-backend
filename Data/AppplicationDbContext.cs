using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using userMangment.Models;
using System.Security.Cryptography.X509Certificates;

namespace userMangment;
public class ApplicationDbContext  : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
        
    }


    public DbSet<GymClass> GymClasses {get; set;}

    
}