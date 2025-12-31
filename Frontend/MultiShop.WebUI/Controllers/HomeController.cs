using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Models.Catalog;
using MultiShop.WebUI.Services;

namespace MultiShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatalogApiClient _catalogApiClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ICatalogApiClient catalogApiClient,
            ILogger<HomeController> logger)
        {
            _catalogApiClient = catalogApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? categoryId = null)
        {
            var viewModel = new CatalogViewModel();

            try
            {
                // Load categories
                viewModel.Categories = await _catalogApiClient.GetCategoriesAsync();

                // Load products (filtered by category if specified)
                if (!string.IsNullOrEmpty(categoryId))
                {
                    viewModel.Products = await _catalogApiClient.GetProductsByCategoryAsync(categoryId);
                    viewModel.SelectedCategoryId = categoryId;
                    viewModel.SelectedCategoryName = viewModel.Categories
                        .FirstOrDefault(c => c.CategoryID == categoryId)?.CategoryName;
                }
                else
                {
                    viewModel.Products = await _catalogApiClient.GetProductsAsync();
                }

                _logger.LogInformation("Loaded {CategoryCount} categories and {ProductCount} products",
                    viewModel.Categories.Count, viewModel.Products.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading catalog data");
                viewModel.ErrorMessage = "Katalog verileri yüklenirken bir hata oluþtu. Lütfen daha sonra tekrar deneyin.";
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Product(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _catalogApiClient.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
