// Backend Integration for FRNCH Jewelry Shop
const API_BASE = '';

// State Management
let backendProducts = [];
let backendCategories = [];
let currentUser = null;
let isAuthenticated = true;  // Always true - no login required
let isAdmin = true;          // Always true - give admin access

// ==================== DATA LOADING ====================

async function loadCategoriesFromBackend() {
    console.log('📦 Loading categories...');
    try {
        const response = await fetch(API_BASE + '/api/catalog/categories');
        console.log('📦 Categories response status:', response.status);
        
        if (response.ok) {
            const categories = await response.json();
            console.log('📦 Categories raw data:', categories);
            
            backendCategories = categories.map(function(cat) {
                return {
                    id: cat.CategoryID || cat.categoryID || cat.categoryId || cat.id,
                    name: cat.CategoryName || cat.categoryName || cat.name || 'Unnamed'
                };
            });
            
            console.log('📦 Categories loaded:', backendCategories.length, backendCategories);
            updateCategoryMenu();
            return true;
        } else {
            const errorText = await response.text();
            console.error('❌ Categories failed:', response.status, errorText);
            return false;
        }
    } catch (error) {
        console.error('❌ Error loading categories:', error);
        return false;
    }
}

async function loadProductsFromBackend() {
    console.log('🛍️ Loading products...');
    try {
        const response = await fetch(API_BASE + '/api/catalog/products');
        console.log('🛍️ Products response status:', response.status);
        
        if (response.ok) {
            const products = await response.json();
            console.log('🛍️ Products raw data:', products);
            
            backendProducts = products.map(function(p, index) {
                var oldPriceVal = p.ProductOldPrice || p.productOldPrice;
                return {
                    id: p.ProductID || p.productID || p.productId || p.id || index + 1,
                    name: p.ProductName || p.productName || p.name || 'Unnamed Product',
                    price: parseFloat(p.ProductPrice || p.productPrice || p.price || 0),
                    oldPrice: oldPriceVal ? parseFloat(oldPriceVal) : null,
                    image: p.ProductImageUrl || p.productImageUrl || 'placeholder',
                    category: p.CategoryID || p.categoryID || p.categoryId || 'all',
                    sku: p.ProductSKU || p.productSKU || 'PRD-' + index,
                    description: p.ProductDescription || p.productDescription || 'No description',
                    badge: determineBadge(p)
                };
            });
            
            console.log('🛍️ Products loaded:', backendProducts.length, backendProducts);
            
            window.products = backendProducts;
            
            if (typeof loadProducts === 'function') {
                loadProducts();
            } else {
                renderProducts();
            }
            
            return true;
        } else {
            var errorText = await response.text();
            console.error('❌ Products failed:', response.status, errorText);
            return false;
        }
    } catch (error) {
        console.error('❌ Error loading products:', error);
        return false;
    }
}

function determineBadge(product) {
    if (product.IsNew || product.isNew) return 'YENİ';
    
    var oldPrice = product.ProductOldPrice || product.productOldPrice;
    var price = product.ProductPrice || product.productPrice || product.price;
    if (oldPrice && price && oldPrice > price) {
        var discount = Math.round(((oldPrice - price) / oldPrice) * 100);
        return '%' + discount + ' İNDİRİM';
    }
    
    if (product.IsBestSeller || product.isBestSeller) return 'ÇOK SATAN';
    
    return null;
}

function updateCategoryMenu() {
    var navMenu = document.querySelector('.nav-menu');
    if (!navMenu) return;
    
    var menuHTML = '';
    
    if (backendCategories.length > 0) {
        backendCategories.slice(0, 4).forEach(function(cat) {
            menuHTML += '<li><a href="#" onclick="filterByCategory(\'' + cat.id + '\')">' + cat.name.toUpperCase() + '</a></li>';
        });
    } else {
        menuHTML = '<li><a href="#" onclick="filterProducts(\'all\')">ERKEK</a></li>' +
                   '<li><a href="#" onclick="filterProducts(\'women\')">KADIN</a></li>' +
                   '<li><a href="#" onclick="filterProducts(\'new\')">SAAT</a></li>' +
                   '<li><a href="#" onclick="filterProducts(\'sale\')">AKSESUAR</a></li>';
    }
    
    menuHTML += '<li><a href="#" onclick="showPage(\'admin\')">ADMIN</a></li>';
    navMenu.innerHTML = menuHTML;
}

function filterByCategory(categoryId) {
    console.log('🔍 Filtering by category:', categoryId);
    var filtered = backendProducts.filter(function(p) { return p.category === categoryId; });
    var productsToShow = filtered.length > 0 ? filtered : backendProducts;
    renderProductGrid(productsToShow);
}

function renderProducts() {
    var productsToShow = backendProducts.length > 0 ? backendProducts : window.products;
    renderProductGrid(productsToShow);
}

function renderProductGrid(productsToShow) {
    var productGrid = document.getElementById('productGrid');
    if (!productGrid) return;
    
    productGrid.innerHTML = '';
    
    productsToShow.forEach(function(product) {
        var badgeHTML = product.badge ? '<div class="product-badge">' + product.badge + '</div>' : '';
        var oldPriceHTML = product.oldPrice ? '<span class="old-price">₺' + formatPrice(product.oldPrice) + '</span>' : '';
        var priceClass = product.oldPrice ? 'new-price' : '';
        
        productGrid.innerHTML += 
            '<div class="product-card fade-in" onclick="showProductDetail(\'' + product.id + '\')">' +
                '<div class="product-image">' +
                    '<div style="background: #1a1a1a; height: 100%; display: flex; align-items: center; justify-content: center; color: #666; padding: 20px; text-align: center;">' +
                        product.name.substring(0, 30) +
                    '</div>' +
                    badgeHTML +
                    '<div class="quick-add" onclick="quickAddToCart(\'' + product.id + '\', event)">SEPETE EKLE</div>' +
                '</div>' +
                '<div class="product-info">' +
                    '<div class="product-name">' + product.name + '</div>' +
                    '<div class="product-price">' +
                        oldPriceHTML +
                        '<span class="' + priceClass + '">₺' + formatPrice(product.price) + '</span>' +
                    '</div>' +
                '</div>' +
            '</div>';
    });
}

                    async function initializeBackend() {
    console.log('🚀 Initializing FRNCH Shop with Backend...');
    await loadCategoriesFromBackend();
    await loadProductsFromBackend();
    console.log('✅ Backend integration complete!');
}

// ==================== PAGE LOAD - NO LOGIN ====================

document.addEventListener('DOMContentLoaded', async () => {
    console.log('🎨 FRNCH Shop Loading (NO LOGIN REQUIRED)...');
    
    // Directly show main shop - skip all login logic
    updateCategoryMenu();
    await initializeBackend();
    
    // Force show mainShop, hide login
    document.querySelectorAll('.page').forEach(p => p.classList.add('hidden'));
    var mainShop = document.getElementById('mainShop');
    if (mainShop) {
        mainShop.classList.remove('hidden');
    }
    
    console.log('✅ Shop loaded - no login required!');
});

// Export for debugging
window.backendIntegration = {
    initializeBackend,
    loadProductsFromBackend,
    loadCategoriesFromBackend,
    backendProducts: () => backendProducts,
    backendCategories: () => backendCategories
};

console.log('✅ Backend integration loaded (NO LOGIN)');