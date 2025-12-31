using MyShopWebSite.Catalog.Dtos.ProductDetailDtos;

namespace MyShopWebSite.Catalog.Services.ProductDetailServices
{
    public interface IProductDetailService
    {
        Task<List<ResultProductDetailDto>> GetAllProductDetailsAsync();
        Task<ResultProductDetailDto> GetProductDetailByIdAsync(string ProductDetailId);
        Task CreateProductDetailAsync(CreateProductDetailDto createProductDetailDto);
        Task UpdateProductDetailAsync(UpdateProductDetailDto updateProductDetailDto);
        Task DeleteProductDetailAsync(string ProductDetailId);
        
    }
}
