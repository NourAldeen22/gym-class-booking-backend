using System.ComponentModel.DataAnnotations;
using userMangment.Models;

namespace userMangment.ViewModels;

public class UserProfileViewModel
{   
    
    public string? UserId{get;set;}
    public string? FullName {get;set;}
    public string? Email{get;set;}
    public bool IsMember {get; set;}
    public List<string> Role { get; set; } = new();
    public List<UserBookingViewModel>? Bookings {get;set;}

    
    
}

public class UserBookingViewModel
    {
        public int BookingId {get;set;}
        public string? ClassName {get;set;}
        public DateTime StartTime {get; set;}
        public DateTime EndTime { get; set;}
        
    public bool IsPast => EndTime < DateTime.UtcNow;
    public bool isLocked => !IsPast && StartTime < DateTime.UtcNow.AddHours(2);
    }



