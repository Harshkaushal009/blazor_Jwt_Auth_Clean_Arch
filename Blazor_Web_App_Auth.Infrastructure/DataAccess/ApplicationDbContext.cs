using Blazor_Web_App_Auth.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor_Web_App_Auth.Infrastructure.DataAccess
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }



    }
}
