using Blazor_Web_App_Auth.Application;
using Blazor_Web_App_Auth.Application.Common;
using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Application.Services.Authentication;
using Blazor_Web_App_Auth.Application.Services.ProductService;
using Blazor_Web_App_Auth.Application.Validations;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Domain.ViewModels;
using Blazor_Web_App_Auth.Infrastructure.DataAccess;
using Blazor_Web_App_Auth.Infrastructure.Repository;
using Blazor_Web_App_Auth_Jwt.Components;
using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var config = builder.Configuration;

#region Datatabase Connection
var cs = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(cs));
#endregion

#region Adding Services and Repositories
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();
builder.Services.AddScoped<IValidator<LoginViewModel>, LoginValidator>();
builder.Services.AddScoped<IValidator<User>, RegisterValidator>();
builder.Services.AddScoped<ITokenHandler,Tokenhandler>();
builder.Services.AddAntiforgery();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl) });
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(Constants.ApiBaseUrl);
});

// Adding services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
#endregion

#region Jwt Configuration
var jwtSettings = config.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType="Role"
        };
    });

#endregion

#region Role Based Auth
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireRole("User"));

    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Admin"));
});

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Blazor_Web_App_Auth_Jwt.Client._Imports).Assembly);

app.Run();
