using Blazor_Web_App_Auth.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> AddProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<bool> UpdateProductAsync(Product product);
    }
}
