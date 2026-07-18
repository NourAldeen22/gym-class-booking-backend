using System.ComponentModel.DataAnnotations;
namespace userMangment.ViewModels;
public class GymClassFormViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Class name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string ClassName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; }
    
    [Required(ErrorMessage = "Duration is required")]
    [Display(Name = "Duration (minutes)")]
    public TimeSpan Duration { get; set; } // بدلاً من TimeSpan
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    // خصائص للعرض
    public string DurationDisplay => Duration.ToString(@"hh\:mm");
}