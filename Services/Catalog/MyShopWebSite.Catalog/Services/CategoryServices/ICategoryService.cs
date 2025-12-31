using MyShopWebSite.Catalog.Dtos.CategoryDtos;

namespace MyShopWebSite.Catalog.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<ResultCategoryDto>> GetAllCategoriesAsync();
        Task<ResultCategoryDto> GetCategoryByIdAsync(string categoryId);
        Task CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(string categoryId);
        
    }
}
