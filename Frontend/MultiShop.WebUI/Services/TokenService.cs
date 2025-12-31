using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Models;
using System.Text.Json;

namespace MultiShop.WebUI.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IdentityServerConfiguration _identityConfig;
        private readonly ILogger<TokenService> _logger;
        private const string TokenCacheKey = "IdentityServerAccessToken";

        public TokenService(
            HttpClient httpClient,
            IMemoryCache cache,
            IOptions<IdentityServerConfiguration> identityConfig,
            ILogger<TokenService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _identityConfig = identityConfig.Value;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Check cache first
            if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                _logger.LogInformation("Using cached access token");
                return cachedToken;
            }

            _logger.LogInformation("Requesting new access token from {TokenUrl}", _identityConfig.TokenUrl);

            // Request new token using client_credentials
            var requestData = new Dictionary<string, string>
            {
                { "client_id", _identityConfig.ClientId },
                { "client_secret", _identityConfig.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scope", _identityConfig.Scopes }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _identityConfig.TokenUrl)
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Token request failed: {StatusCode} - {Content}", response.StatusCode, responseContent);
                    throw new HttpRequestException($"Token request failed: {response.StatusCode}");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    _logger.LogError("Invalid token response received");
                    throw new InvalidOperationException("Invalid token response");
                }

                // Cache token (expire 60 seconds before actual expiry for safety)
                var cacheExpiry = TimeSpan.FromSeconds(Math.Max(tokenResponse.ExpiresIn - 60, 60));
                _cache.Set(TokenCacheKey, tokenResponse.AccessToken, cacheExpiry);

                _logger.LogInformation("Access token acquired successfully, expires in {ExpiresIn}s", tokenResponse.ExpiresIn);
                return tokenResponse.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquiring access token");
                throw;
            }
        }
    }
}