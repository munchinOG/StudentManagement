using Microsoft.AspNetCore.Identity;

namespace Student.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
