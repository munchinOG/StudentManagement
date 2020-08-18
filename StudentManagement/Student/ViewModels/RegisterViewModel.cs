using Microsoft.AspNetCore.Mvc;
using Student.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Student.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote( action: "IsEmailInUse", controller: "Account" )]
        [ValidEmailDomain( allowedDomain: "munchintech.com",
            ErrorMessage = "Email domain must be munchintech.com" )]
        public string Email { get; set; }
        [Required]
        [DataType( DataType.Password )]
        public string Password { get; set; }
        [DataType( DataType.Password )]
        [Display( Name = "Confirm Password" )]
        [Compare( "Password", ErrorMessage = "Password and confirmation password do not match." )]
        public string ConfirmPassword { get; set; }
    }
}
