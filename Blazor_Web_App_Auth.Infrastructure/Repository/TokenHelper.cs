using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Infrastructure.Repository
{
    public class Tokenhandler : ITokenHandler
    {
        private readonly ApplicationDbContext _dbContext;

        public Tokenhandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveRefreshTokenAsync(UserRefreshTokens userRefreshToken)
        {
            _dbContext.UserRefreshTokens.Add(userRefreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            return await _dbContext.UserRefreshTokens
                .AnyAsync(t => t.UserId == userId && t.RefreshToken == refreshToken && !t.IsRevoked);
        }

        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await _dbContext.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.RefreshToken == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
