using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Domain.Models
{
    public class UserRefreshTokens
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? RevokedDate { get; set; }

        public bool IsRevoked { get; set; } = false;
    }
}
