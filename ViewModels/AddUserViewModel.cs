using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace userMangment.ViewModels;

public class AddUserViewModel
{
    [Required(ErrorMessage ="First name required.")]
    public string FirstName{get; set;} = string.Empty;
    
    public string LastName {get; set;} = string.Empty;

    [Required(ErrorMessage = "Email required")]
    [EmailAddress(ErrorMessage = "The email format is incorrect.")]
    public string Email {get;set;} = string.Empty;

    [Required(ErrorMessage ="Password required.")]
    [DataType(DataType.Password)]
    public string Password{get; set;} = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage ="The password does not match.")]
     public string ConfirmPassword{get; set;} = string.Empty;


// public AddUserViewModel(string firstName, string lastName , string email , string password , string confirmPassword)
// {
//     this.FirstName = firstName;
//     this.LastName = lastName;
//     this.Email = email;     
//     this.Password = password;
//     this.ConfirmPassword = confirmPassword;   
// }

// public AddUserViewModel()
// {
// }

}