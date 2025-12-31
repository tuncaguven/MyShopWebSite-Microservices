using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Models;

namespace MultiShop.WebUI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IdentityServerConfiguration _identityConfig;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IOptions<IdentityServerConfiguration> identityOptions,
            ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _identityConfig = identityOptions.Value;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login validation failed: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            // Step 1: Verify user with IdentityServer Login endpoint
            var loginUrl = $"{_identityConfig.Authority.TrimEnd('/')}/api/Login";
            
            var loginPayload = new
            {
                username = request.Username,
                password = request.Password
            };

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(loginPayload);
                using var loginContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var loginResponse = await httpClient.PostAsync(loginUrl, loginContent);
                var loginResponseText = await loginResponse.Content.ReadAsStringAsync();

                if (!loginResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Login failed for {Username}. Response: {Response}", request.Username, loginResponseText);
                    return Unauthorized(new { error = "login_failed", message = loginResponseText });
                }

                _logger.LogInformation("Login verified for {Username}, requesting token...", request.Username);

                // Step 2: Get token from IdentityServer
                var clientId = string.IsNullOrWhiteSpace(_identityConfig.UserClientId)
                    ? _identityConfig.ClientId
                    : _identityConfig.UserClientId;

                var clientSecret = string.IsNullOrWhiteSpace(_identityConfig.UserClientSecret)
                    ? _identityConfig.ClientSecret
                    : _identityConfig.UserClientSecret;

                var scopes = string.IsNullOrWhiteSpace(_identityConfig.UserScopes)
                    ? _identityConfig.Scopes
                    : _identityConfig.UserScopes;

                var tokenPayload = new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "password",
                    ["username"] = request.Username,
                    ["password"] = request.Password,
                    ["scope"] = scopes
                };

                using var tokenContent = new FormUrlEncodedContent(tokenPayload);
                var tokenResponse = await httpClient.PostAsync(_identityConfig.TokenUrl, tokenContent);
                var tokenResponseText = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Token request failed. Response: {Response}", tokenResponseText);
                    return StatusCode((int)tokenResponse.StatusCode, tokenResponseText);
                }

                _logger.LogInformation("Token acquired successfully for {Username}", request.Username);
                return Content(tokenResponseText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Username}", request.Username);
                return StatusCode(500, new { error = "server_error", message = "Giriş işlemi sırasında bir hata oluştu." });
            }
        }
    }
}