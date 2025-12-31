using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopWebSite.Discount.Services;

namespace MyShopWebSite.Discount.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public DiscountController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCouponAsync()
        {
            var coupons = await _couponService.GetAllCouponAsync();
            return Ok(coupons);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Dtos.CreateCouponDto createCouponDto)
        {
            await _couponService.CreateAsync(createCouponDto);
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            return Ok(coupon);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _couponService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] Dtos.UpdateCouponDto updateCouponDto)
        {
            await _couponService.UpdateAsync(updateCouponDto);
            return Ok();
        }
    }
}
