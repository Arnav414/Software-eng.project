using System.ComponentModel.DataAnnotations;

namespace ClinicianPortal.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "First name required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Last name required")]
        public string? LastName { get; set; }
    }
}
