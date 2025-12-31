using MultiShop.WebUI.Models.Catalog;

namespace MultiShop.WebUI.Services
{
    public interface ICatalogApiClient
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<ProductDto>> GetProductsAsync();
        Task<List<ProductDto>> GetProductsByCategoryAsync(string categoryId);
        Task<ProductDto?> GetProductByIdAsync(string productId);
    }
}