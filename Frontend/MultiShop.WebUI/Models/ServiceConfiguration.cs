namespace MultiShop.WebUI.Models
{
    public class IdentityServerConfiguration
    {
        public string Authority { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Scopes { get; set; } = string.Empty;
        public string UserClientId { get; set; } = string.Empty;
        public string UserClientSecret { get; set; } = string.Empty;
        public string UserScopes { get; set; } = string.Empty;
    }

    public class ServiceUrlConfiguration
    {
        public string Catalog { get; set; } = string.Empty;
        public string Basket { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
        public string Discount { get; set; } = string.Empty;
    }
}