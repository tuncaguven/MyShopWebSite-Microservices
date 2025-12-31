namespace MultiShop.WebUI.Models.Catalog
{
    public class ProductDto
    {
        public string ProductID { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string CategoryID { get; set; } = string.Empty;
    }
}