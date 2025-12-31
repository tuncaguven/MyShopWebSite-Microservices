using MyShopWebSite.Catalog.Dtos.ProductImageDtos;

namespace MyShopWebSite.Catalog.Services.ProductImageServices
{
    public interface IProductImageService
    {
        Task<List<ResultProductImageDto>> GetAllProductImagesAsync();
        Task<ResultProductImageDto> GetProductImageByIdAsync(string ProductImageId);
        Task CreateProductImageAsync(CreateProductImageDto createProductImageDto);
        Task UpdateProductImageAsync(UpdateProductImageDto updateProductImageDto);
        Task DeleteProductImageAsync(string ProductImageId);
        
    }
}
