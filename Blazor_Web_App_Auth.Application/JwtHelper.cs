using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenHandler _tokenHandler;

        public JwtHelper(IConfiguration configuration, ITokenHandler tokenHandler)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenHandler = tokenHandler ?? throw new ArgumentNullException(nameof(tokenHandler));
        }

        public string GenerateToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
           
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            var userRefreshToken = new UserRefreshTokens
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiryMinutes"])),
            };

            await _tokenHandler.SaveRefreshTokenAsync(userRefreshToken);
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            return await _tokenHandler.ValidateRefreshTokenAsync(userId, refreshToken);
        }

        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            await _tokenHandler.RevokeRefreshTokenAsync(userId, refreshToken);
        }

        public async Task<string> GenerateNewAccessTokenAsync(string expiredToken, string refreshToken)
        {
            if (string.IsNullOrEmpty(expiredToken)) throw new ArgumentNullException(nameof(expiredToken));
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            var principal = GetPrincipalFromExpiredToken(expiredToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !await ValidateRefreshTokenAsync(int.Parse(userId), refreshToken))
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newAccessToken = GenerateToken(new User
            {
                Id = int.Parse(userId),
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                Name = principal.FindFirst(ClaimTypes.Name)?.Value,
                Role = principal.FindFirst(ClaimTypes.Role)?.Value
            });

            return newAccessToken;
        }
    }
}
