namespace userMangment.ViewModels;

public class GymClassViewModel
{
    public int Id{get;  set;}

    public string ClasName {get; set;} = string.Empty;

    public string Description  {get; set;} = string.Empty;

    public DateTime StartTime{get; set;}

  

    public TimeSpan Duration {get;set;}
    public DateTime EndTime => StartTime + Duration;

    public string StartTimeDisplay => StartTime.ToString("d-M-yyyy hh:mm tt");
    public string StartDateDisplay => StartTime.ToString("dddd, MMMM dd, yyyy");
    public string StartTimeOnly => StartTime.ToString("hh:mm tt");
    public string DurationDisplay => Duration.ToString(@"hh\:mm");
    public string EndTimeDisplay => (StartTime + Duration).ToString("hh:mm tt");

    public bool IsBookingEnd => DateTime.UtcNow >= EndTime;
    public bool IsClassStart => DateTime.Now >= StartTime && DateTime.Now< EndTime;
    public bool isUnBookLocked => StartTime.Subtract(DateTime.Now).TotalHours < 2;

    public bool ISBokingByUser;
    public List<GymClass> ClassFilter { get; set; } = new();

    public int MemberCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}