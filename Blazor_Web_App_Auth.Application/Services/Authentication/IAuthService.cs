using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginViewModel request);
        Task<bool> RegisterAsync(User user);

        Task LogoutAsync();
    }

}
