using Microsoft.Extensions.Options;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Models.Catalog;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MultiShop.WebUI.Services
{
    public class CatalogApiClient : ICatalogApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly ILogger<CatalogApiClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CatalogApiClient(
            HttpClient httpClient,
            ITokenService tokenService,
            IOptions<ServiceUrlConfiguration> serviceUrls,
            ILogger<CatalogApiClient> logger)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(serviceUrls.Value.Catalog.TrimEnd('/') + "/");
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.GetAsync("api/Categories");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch categories: {StatusCode}", response.StatusCode);
                    return new List<CategoryDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CategoryDto>>(content, JsonOptions) ?? new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return new List<CategoryDto>();
            }
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.GetAsync("api/Products");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch products: {StatusCode}", response.StatusCode);
                    return new List<ProductDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ProductDto>>(content, JsonOptions) ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return new List<ProductDto>();
            }
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(string categoryId)
        {
            var allProducts = await GetProductsAsync();
            return allProducts.Where(p => p.CategoryID == categoryId).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(string productId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.GetAsync($"api/Products/{productId}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(content, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product {ProductId}", productId);
                return null;
            }
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}