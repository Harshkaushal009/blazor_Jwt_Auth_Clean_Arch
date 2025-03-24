
namespace Blazor_Web_App_Auth.Domain.Models
{
    public class LoginResponse
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }
    }
}
