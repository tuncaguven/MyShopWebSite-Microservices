using MyShopWebSite.Basket.Dtos;

namespace MyShopWebSite.Basket.Services
{
    public interface IBasketService
    {
        Task<BasketTotalDto> GetBasket(string userId);

        Task SaveBasket(BasketTotalDto basketTotalDto);

        Task DeleteBasket(string userId);

    }
}
