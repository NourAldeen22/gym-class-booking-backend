namespace userMangment.Models;


public class GymClass
{
    
    public int Id {get; set;}
    public string? Name {get; set;}
    public DateTime StartTime {get;set;}
    public TimeSpan Duration {get; set;}
    public DateTime EndTime {get { return StartTime + Duration; }}
    public string? Desctiption {get;set;}

    public virtual ICollection<ApplicationUser> AttendingMembers {get; set;} = new List<ApplicationUser>();




    public bool BookTime()
    {
               
       var currentTime = DateTime.UtcNow;

       if(currentTime >= StartTime)
        {
            
            return false;
        }

        //  Console.WriteLine($"You have booked the class {Name} at {StartTime} for {Duration}");
         return true;

    }

    public bool IsBookingClosed()
    {
        return DateTime.UtcNow >= StartTime;
    }

    public bool IsForbiddenCancle()
    {
        return DateTime.UtcNow >= StartTime.AddHours(-2);
        
    }

    public bool AvilabileBooking()
    {
        return  EndTime >= DateTime.UtcNow;
    }


    public bool IsUppcoming() => StartTime >= DateTime.UtcNow;
   
    
    
  
     


}
