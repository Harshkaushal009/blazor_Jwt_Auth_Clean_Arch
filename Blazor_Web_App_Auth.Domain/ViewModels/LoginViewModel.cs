using System.ComponentModel.DataAnnotations;

namespace Blazor_Web_App_Auth.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
