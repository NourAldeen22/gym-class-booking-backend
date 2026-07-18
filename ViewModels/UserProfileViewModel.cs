using System.ComponentModel.DataAnnotations;
using userMangment.Models;

namespace userMangment.ViewModels;

public class UserProfileViewModel
{   
    
    public string? UserId{get;set;}
    public string? FullName {get;set;}
    public string? Email{get;set;}
    public bool IsMember {get; set;}
    public List<UserBookingViewModel>? Bookings {get;set;}

    
    
}

public class UserBookingViewModel
    {
        public int BookingId {get;set;}

        public string? ClassName {get;set;}

        public DateTime ClassTime {get; set;}
    }



