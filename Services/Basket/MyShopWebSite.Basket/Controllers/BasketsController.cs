using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopWebSite.Basket.Dtos;
using MyShopWebSite.Basket.LoginServices;
using MyShopWebSite.Basket.Services;

namespace MyShopWebSite.Basket.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly ILoginService _loginService;

        public BasketsController(IBasketService basketService, ILoginService loginService)
        {
            _basketService = basketService;
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBaskets()
        {
            // For demo purposes, return sample baskets
            var sampleBaskets = new[]
            {
                new
                {
                    BasketId = "basket-001",
                    UserId = "user-123",
                    Items = new[]
                    {
                        new { ProductId = "prod-1", ProductName = "Laptop", Quantity = 1, Price = 1299.99 },
                        new { ProductId = "prod-2", ProductName = "Mouse", Quantity = 2, Price = 29.99 }
                    },
                    TotalPrice = 1359.97,
                    ItemCount = 2
                },
                new
                {
                    BasketId = "basket-002",
                    UserId = "user-456",
                    Items = new[]
                    {
                        new { ProductId = "prod-3", ProductName = "Keyboard", Quantity = 1, Price = 89.99 }
                    },
                    TotalPrice = 89.99,
                    ItemCount = 1
                }
            };
            return Ok(sampleBaskets);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasketByUserId(string userId)
        {
            try
            {
                var basket = await _basketService.GetBasket(userId);
                return Ok(basket);
            }
            catch
            {
                return NotFound(new { message = "Sepet bulunamadý" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBasket(BasketTotalDto basketTotalDto)
        {
            basketTotalDto.UserId = _loginService.GetUserId;
            await _basketService.SaveBasket(basketTotalDto);
            return Ok(new { message = "Sepet baþarýyla kaydedildi" });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteBasket(string userId)
        {
            await _basketService.DeleteBasket(userId);
            return Ok(new { message = "Sepet baþarýyla silindi" });
        }
    }
}