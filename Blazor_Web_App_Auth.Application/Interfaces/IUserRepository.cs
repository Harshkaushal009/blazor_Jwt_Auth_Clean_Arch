using Blazor_Web_App_Auth.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task AddUserAsync(User user);

    }
}
