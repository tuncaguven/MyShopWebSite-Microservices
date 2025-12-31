using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services;
using System.Net.Http.Headers;

namespace MultiShop.WebUI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiProxyController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceUrlConfiguration _serviceUrls;
        private readonly ILogger<ApiProxyController> _logger;

        public ApiProxyController(
            ITokenService tokenService,
            IHttpClientFactory httpClientFactory,
            IOptions<ServiceUrlConfiguration> serviceUrls,
            ILogger<ApiProxyController> logger)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
            _serviceUrls = serviceUrls.Value;
            _logger = logger;
        }

        // CATALOG ROUTES
        [HttpGet("catalog/categories")]
        public async Task<IActionResult> GetCategories()
        {
            return await ProxyRequest("Catalog", _serviceUrls.Catalog, "api/Categories");
        }

        [HttpGet("catalog/products")]
        public async Task<IActionResult> GetProducts()
        {
            return await ProxyRequest("Catalog", _serviceUrls.Catalog, "api/Products");
        }

        [HttpGet("catalog/products/{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            return await ProxyRequest("Catalog", _serviceUrls.Catalog, $"api/Products/{id}");
        }

        [HttpGet("catalog/health/secure")]
        public async Task<IActionResult> CatalogHealthCheck()
        {
            return await ProxyRequest("Catalog", _serviceUrls.Catalog, "api/health/secure");
        }

        // BASKET ROUTES
        [HttpGet("basket/baskets")]
        public async Task<IActionResult> GetBaskets()
        {
            return await ProxyRequest("Basket", _serviceUrls.Basket, "api/Baskets");
        }

        [HttpGet("basket/baskets/{userId}")]
        public async Task<IActionResult> GetBasket(string userId)
        {
            return await ProxyRequest("Basket", _serviceUrls.Basket, $"api/Baskets/{userId}");
        }

        [HttpPost("basket/baskets")]
        public async Task<IActionResult> SaveBasket()
        {
            return await ProxyRequest("Basket", _serviceUrls.Basket, "api/Baskets", HttpMethod.Post);
        }

        [HttpGet("basket/health/secure")]
        public async Task<IActionResult> BasketHealthCheck()
        {
            return await ProxyRequest("Basket", _serviceUrls.Basket, "api/health/secure");
        }

        // ORDER ROUTES
        [HttpGet("order/orders")]
        public async Task<IActionResult> GetOrders()
        {
            return await ProxyRequest("Order", _serviceUrls.Order, "api/Orders");
        }

        [HttpGet("order/orders/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            return await ProxyRequest("Order", _serviceUrls.Order, $"api/Orders/{id}");
        }

        [HttpPost("order/orders")]
        public async Task<IActionResult> CreateOrder()
        {
            return await ProxyRequest("Order", _serviceUrls.Order, "api/Orders", HttpMethod.Post);
        }

        [HttpGet("order/health/secure")]
        public async Task<IActionResult> OrderHealthCheck()
        {
            return await ProxyRequest("Order", _serviceUrls.Order, "api/health/secure");
        }

        // DISCOUNT ROUTES
        [HttpGet("discount/discounts")]
        public async Task<IActionResult> GetDiscounts()
        {
            return await ProxyRequest("Discount", _serviceUrls.Discount, "api/Discounts");
        }

        [HttpGet("discount/discounts/{id}")]
        public async Task<IActionResult> GetDiscount(int id)
        {
            return await ProxyRequest("Discount", _serviceUrls.Discount, $"api/Discounts/{id}");
        }

        [HttpGet("discount/health/secure")]
        public async Task<IActionResult> DiscountHealthCheck()
        {
            return await ProxyRequest("Discount", _serviceUrls.Discount, "api/health/secure");
        }

        // PROXY REQUEST METHOD
        private async Task<IActionResult> ProxyRequest(
            string serviceName,
            string baseUrl,
            string path,
            HttpMethod? method = null)
        {
            try {
                // Get access token
                var token = await _tokenService.GetAccessTokenAsync();

                // Build target URL
                var targetUrl = $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
                if (Request.QueryString.HasValue)
                {
                    targetUrl += Request.QueryString.Value;
                }

                _logger.LogInformation("Proxying request to {Service}: {Method} {Url}", 
                    serviceName, method?.Method ?? "GET", targetUrl);

                // Create HTTP client
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Create request
                var requestMethod = method ?? HttpMethod.Get;
                var request = new HttpRequestMessage(requestMethod, targetUrl);

                // Copy request body for POST requests
                if (requestMethod == HttpMethod.Post && Request.ContentLength > 0)
                {
                    var bodyContent = await new StreamReader(Request.Body).ReadToEndAsync();
                    request.Content = new StringContent(bodyContent, System.Text.Encoding.UTF8, Request.ContentType ?? "application/json");
                }

                // Send request
                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("{Service} responded with {StatusCode}", serviceName, response.StatusCode);

                // Return response
                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying to {Service}", serviceName);
                return StatusCode(503, new { error = $"Service {serviceName} unavailable", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying request to {Service}", serviceName);
                return StatusCode(500, new { error = "Internal proxy error", message = ex.Message });
            }
        }
    }
}