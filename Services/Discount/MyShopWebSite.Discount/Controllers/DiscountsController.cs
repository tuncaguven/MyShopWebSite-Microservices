using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopWebSite.Discount.Services;

namespace MyShopWebSite.Discount.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public DiscountsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var coupons = await _couponService.GetAllCouponAsync();
            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] Dtos.CreateCouponDto createCouponDto)
        {
            await _couponService.CreateAsync(createCouponDto);
            return Ok(new { message = "Kupon baþarýyla oluþturuldu" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiscount([FromBody] Dtos.UpdateCouponDto updateCouponDto)
        {
            await _couponService.UpdateAsync(updateCouponDto);
            return Ok(new { message = "Kupon baþarýyla güncellendi" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            await _couponService.DeleteAsync(id);
            return Ok(new { message = "Kupon baþarýyla silindi" });
        }
    }
}