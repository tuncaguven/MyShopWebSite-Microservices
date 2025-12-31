namespace MyShopWebSite.Basket.Settings
{
    public class RedisSettings : IRedisSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
