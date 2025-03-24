using Blazor_Web_App_Auth.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Interfaces
{
    public interface ITokenHandler
    {
        Task SaveRefreshTokenAsync(UserRefreshTokens userRefreshToken);
        Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);
        Task RevokeRefreshTokenAsync(int userId, string refreshToken);
    }
}
