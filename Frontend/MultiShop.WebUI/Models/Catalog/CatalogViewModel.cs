namespace MultiShop.WebUI.Models.Catalog
{
    public class CatalogViewModel
    {
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ProductDto> Products { get; set; } = new();
        public string? SelectedCategoryId { get; set; }
        public string? SelectedCategoryName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}