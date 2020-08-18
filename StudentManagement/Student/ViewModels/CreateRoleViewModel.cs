using System.ComponentModel.DataAnnotations;

namespace Student.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
