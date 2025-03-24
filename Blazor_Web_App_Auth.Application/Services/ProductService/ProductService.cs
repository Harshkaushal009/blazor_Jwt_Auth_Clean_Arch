using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            try
            {
                bool result = await _productRepository.AddProductAsync(product);
                _logger.LogInformation("Product added: {ProductId}", product.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product: {ProductName}", product.Name);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID", nameof(id));

            try
            {
                bool result = await _productRepository.DeleteProductAsync(id);
                _logger.LogInformation("Product deleted: {ProductId}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product: {ProductId}", id);
                return false;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID", nameof(id));

            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _productRepository.GetProductsAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            try
            {
                bool result = await _productRepository.UpdateProductAsync(product);
                _logger.LogInformation("Product updated: {ProductId}", product.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product: {ProductId}", product.Id);
                return false;
            }
        }
    }
}
