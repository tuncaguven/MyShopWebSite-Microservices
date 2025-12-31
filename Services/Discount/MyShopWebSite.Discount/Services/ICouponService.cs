using MyShopWebSite.Discount.Dtos;

namespace MyShopWebSite.Discount.Services
{
    public interface ICouponService
    {
        public Task<List<ResultCouponDto>> GetAllCouponAsync();
        public Task CreateAsync(CreateCouponDto createCouponDto);
        public Task<ResultCouponDto> GetByIdAsync(int id);
        public Task DeleteAsync(int id);
        public Task UpdateAsync(UpdateCouponDto updateCouponDto);
    }
}
