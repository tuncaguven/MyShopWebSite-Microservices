using MyShopWebSite.Catalog.Dtos.ProductDtos;

namespace MyShopWebSite.Catalog.Services.ProductServices
{
    public interface IProductService
    {
        Task<List<ResultProductDto>> GetAllProductsAsync();
        Task<ResultProductDto> GetProductByIdAsync(string productId);
        Task CreateProductAsync(CreateProductDto createProductDto);
        Task UpdateProductAsync(UpdateProductDto updateProductDto);
        Task DeleteProductAsync(string ProductId);
    }
}
