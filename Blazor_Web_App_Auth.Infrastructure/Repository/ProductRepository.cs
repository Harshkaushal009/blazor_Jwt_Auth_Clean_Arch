using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blazor_Web_App_Auth.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product added successfully: {ProductId}", product.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product: {ProductName}", product.Name);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID", nameof(id));

            try
            {
                var productToDelete = await _context.Products.FindAsync(id);
                if (productToDelete == null)
                {
                    _logger.LogWarning("Product not found for deletion: {ProductId}", id);
                    return false;
                }

                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product deleted successfully: {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {ProductId}", id);
                return false;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID", nameof(id));

            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product not found for update: {ProductId}", product.Id);
                    return false;
                }

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductId}", product.Id);
                return false;
            }
        }
    }
}
