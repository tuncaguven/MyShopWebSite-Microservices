using System.Security.Claims;

namespace MyShopWebSite.Basket.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public LoginService(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

            public string GetUserId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available");
                }

                var user = httpContext.User;
                if (user?.Identity?.IsAuthenticated != true)
                {
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                // With claim mapping cleared, "sub" should be preserved
                var subClaim = user.FindFirst("sub");
                
                if (subClaim == null)
                {
                    // Fallback to NameIdentifier just in case
                    subClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                }

                if (subClaim == null)
                {
                    var availableClaims = string.Join(", ", user.Claims.Select(c => c.Type));
                    throw new InvalidOperationException(
                        $"'sub' claim not found. Available claims: {availableClaims}");
                }

                return subClaim.Value;
            }
        }
    }
}
