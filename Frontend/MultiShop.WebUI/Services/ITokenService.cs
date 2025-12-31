namespace MultiShop.WebUI.Services
{
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}