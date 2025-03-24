using System.ComponentModel.DataAnnotations;

namespace Blazor_Web_App_Auth.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
    }
}
