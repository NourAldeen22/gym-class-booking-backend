namespace userMangment.ViewModels;

public class UserForAdminViewModel
{
    public string? UserId{get;set;}
    public string? FullName{get;set;}
    public string? Email {get;set;}
    public bool IsMember{get;set;}
    public int TotalBookingsCount{get;set;}
}

public class AdminDashboardViewModel
{
    public List<UserForAdminViewModel> AllUsers {get; set;} = new();

    public int TotalUsersInSystem{get; set;}
    public int UpcomingClassesCount{get;set;}

    
}