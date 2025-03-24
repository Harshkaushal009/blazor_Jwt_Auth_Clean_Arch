using Blazor_Web_App_Auth.Application.Common;
using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Domain.ViewModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Blazor_Web_App_Auth.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ITokenHandler _tokenhandler;

        public AuthService(HttpClient httpClient,ITokenHandler tokenHandler, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _tokenhandler = tokenHandler;
        }

        public async Task<bool> LoginAsync(LoginViewModel request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse is null || string.IsNullOrEmpty(loginResponse.Token) || string.IsNullOrEmpty(loginResponse.RefreshToken))
                return false;

            await _localStorage.SetItemAsync(Constants.JwtTokenKey, loginResponse.Token);
            var refreshToken = new UserRefreshTokens
            {
                RefreshToken = loginResponse.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(60)
            };
            await _tokenhandler.SaveRefreshTokenAsync(refreshToken);
            await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginResponse.Token);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(Constants.JwtTokenKey);
            await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<bool> RegisterAsync(User user)
        {
          var registerReposne=await _httpClient.PostAsJsonAsync("api/auth/register", user);
            if (!registerReposne.IsSuccessStatusCode)
                return false;
            return true;
        }
    }
}
