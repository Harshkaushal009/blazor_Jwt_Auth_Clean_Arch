using Blazor_Web_App_Auth.Application.Common;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Services.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(Constants.JwtTokenKey);

            var identity = string.IsNullOrEmpty(token) || IsTokenExpired(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token), Constants.AuthScheme);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsync(Constants.JwtTokenKey, token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(Constants.JwtTokenKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static bool IsTokenExpired(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim != null && long.TryParse(expClaim, out var exp))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                return expDate < DateTime.UtcNow;
            }
            return true;
        }


        private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64.Replace('-', '+').Replace('_', '/'));
        }

      
    }
}
