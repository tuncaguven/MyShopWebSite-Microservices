// Backend Integration for FRNCH Jewelry Shop
const API_BASE = ''; // MVC proxy handles everything

// State Management
let backendProducts = [];
let backendCategories = [];
let currentUser = null;
let isAuthenticated = false;
let isAdmin = false;

// Initialize Backend Integration
async function initializeBackend() {
    console.log('🚀 Initializing FRNCH Shop with Backend...');
    
    try {
        // Check if user is logged in
        checkAuthStatus();
        
        // Load categories from Catalog service
        await loadCategoriesFromBackend();
        
        // Load products from Catalog service
        await loadProductsFromBackend();
        
        // Update UI based on auth status
        updateAuthUI();
        
        console.log('✅ Backend integration complete!');
    } catch (error) {
        console.error('❌ Backend initialization failed:', error);
        console.log('Using demo data as fallback...');
    }
}

// Check Authentication Status
function checkAuthStatus() {
    const userStr = localStorage.getItem('frnch_user');
    const token = localStorage.getItem('frnch_token');
    
    if (userStr && token) {
        try {
            currentUser = JSON.parse(userStr);
            isAuthenticated = true;
            isAdmin = currentUser.role === 'Admin' || currentUser.email?.includes('admin');
            console.log('✅ User authenticated:', currentUser.email, '| Admin:', isAdmin);
        } catch (error) {
            console.error('Error parsing user data:', error);
            clearAuthData();
        }
    } else {
        isAuthenticated = false;
        isAdmin = false;
    }
}

// Update Auth UI
function updateAuthUI() {
    // Update login/profile button
    const loginBtn = document.querySelector('.icon-btn[onclick="showPage(\'login\')"]');
    if (loginBtn) {
        if (isAuthenticated) {
            loginBtn.innerHTML = '👤';
            loginBtn.title = `Logged in as: ${currentUser.email}`;
            loginBtn.onclick = showUserMenu;
        } else {
            loginBtn.innerHTML = '👤';
            loginBtn.title = 'Login';
            loginBtn.onclick = function() { showPage('login'); };
        }
    }
    
    // CRITICAL: Hide/show admin menu item based on login status
    updateAdminMenuVisibility();
}

// Update Admin Menu Visibility
function updateAdminMenuVisibility() {
    const navMenu = document.querySelector('.nav-menu');
    if (!navMenu) return;
    
    // Find or create admin menu item
    let adminMenuItem = Array.from(navMenu.querySelectorAll('li')).find(li => 
        li.textContent.includes('ADMIN')
    );
    
    if (isAdmin) {
        // Show admin menu if user is admin
        if (!adminMenuItem) {
            adminMenuItem = document.createElement('li');
            adminMenuItem.innerHTML = '<a href="#" onclick="showPage(\'admin\')">ADMIN</a>';
            navMenu.appendChild(adminMenuItem);
        }
        adminMenuItem.style.display = 'block';
    } else {
        // Hide admin menu if not admin
        if (adminMenuItem) {
            adminMenuItem.style.display = 'none';
        }
    }
}

// Show User Menu
function showUserMenu(event) {
    event.stopPropagation();
    
    const menu = document.createElement('div');
    menu.style.cssText = `
        position: absolute;
        top: 60px;
        right: 40px;
        background: white;
        border: 1px solid #ddd;
        border-radius: 8px;
        box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        padding: 10px 0;
        min-width: 200px;
        z-index: 3000;
    `;
    
    menu.innerHTML = `
        <div style="padding: 15px; border-bottom: 1px solid #eee;">
            <div style="font-weight: bold;">${currentUser.email}</div>
            <div style="font-size: 12px; color: #666;">${isAdmin ? 'Administrator' : 'Customer'}</div>
        </div>
        ${isAdmin ? `
        <div style="padding: 10px 15px; cursor: pointer; transition: background 0.2s;" 
             onmouseover="this.style.background='#f5f5f5'" 
             onmouseout="this.style.background='white'" 
             onclick="showPage('admin')">
            ⚙️ Admin Panel
        </div>
        ` : ''}
        <div style="padding: 10px 15px; cursor: pointer; transition: background 0.2s;" 
             onmouseover="this.style.background='#f5f5f5'" 
             onmouseout="this.style.background='white'" 
             onclick="logout()">
            🚪 Logout
        </div>
    `;
    
    document.body.appendChild(menu);
    
    // Close menu on outside click
    setTimeout(() => {
        document.addEventListener('click', function closeMenu() {
            menu.remove();
            document.removeEventListener('click', closeMenu);
        });
    }, 100);
}

// Logout
function logout() {
    clearAuthData();
    showNotification('Çıkış yapıldı');
    updateAuthUI();
    showPage('mainShop');
    location.reload();
}

// Clear Auth Data
function clearAuthData() {
    localStorage.removeItem('frnch_user');
    localStorage.removeItem('frnch_token');
    currentUser = null;
    isAuthenticated = false;
    isAdmin = false;
}

// Load Categories from Catalog Service
async function loadCategoriesFromBackend() {
    try {
        const response = await fetch(`${API_BASE}/api/catalog/categories`);
        if (response.ok) {
            const categories = await response.json();
            backendCategories = categories;
            console.log('📦 Categories loaded:', backendCategories.length);
            
            if (backendCategories.length > 0) {
                updateCategoryMenu();
            }
        }
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

// Update Category Menu
function updateCategoryMenu() {
    const navMenu = document.querySelector('.nav-menu');
    if (!navMenu) return;
    
    const categoryItems = backendCategories.slice(0, 4).map(cat => `
        <li><a href="#" onclick="filterProductsByBackendCategory('${cat.categoryId || cat.id}')">${(cat.categoryName || cat.name).toUpperCase()}</a></li>
    `).join('');
    
    // Only add admin menu if user is admin
                const adminItem = isAdmin ? '<li><a href="#" onclick="showPage(\'admin\')">ADMIN</a></li>' : '';
    
    navMenu.innerHTML = categoryItems + adminItem;
}

// Load Products from Catalog Service
async function loadProductsFromBackend() {
    try {
        const response = await fetch(`${API_BASE}/api/catalog/products`);
        if (response.ok) {
            const productsData = await response.json();
            
            backendProducts = productsData.map((p, index) => ({
                id: p.productId || p.id || index + 1,
                name: p.productName || p.name || 'Unnamed Product',
                price: parseFloat(p.productPrice || p.price || 0),
                oldPrice: p.productOldPrice ? parseFloat(p.productOldPrice) : null,
                image: p.productImageUrl || 'placeholder',
                badge: determineBadge(p),
                category: p.categoryId || 'all',
                sku: p.productSKU || `PRD-${p.productId || index}`,
                description: p.productDescription || 'No description available'
            }));
            
            console.log('🛍️ Products loaded:', backendProducts.length);
            
            if (backendProducts.length > 0) {
                window.products = backendProducts;
                if (typeof loadProducts === 'function') {
                    loadProducts();
                }
            }
        }
    } catch (error) {
        console.error('Error loading products:', error);
    }
}

// Determine Badge
function determineBadge(product) {
    if (product.isNew || product.productBadge === 'new') return 'YENİ';
    if (product.productOldPrice && product.productPrice) {
        const discount = Math.round(((product.productOldPrice - product.productPrice) / product.productOldPrice) * 100);
        if (discount > 0) return `%${discount} İNDİRİM`;
    }
    if (product.isBestSeller) return 'ÇOK SATAN';
    return null;
}

// Filter Products by Backend Category
function filterProductsByBackendCategory(categoryId) {
    const filtered = backendProducts.filter(p => p.category === categoryId);
    const productsToShow = filtered.length > 0 ? filtered : backendProducts;
    
    const productGrid = document.getElementById('productGrid');
    if (!productGrid) return;
    
    productGrid.innerHTML = '';
    
    productsToShow.forEach(product => {
        productGrid.innerHTML += createProductCard(product);
    });
    
    // Update active button
    document.querySelectorAll('.nav-menu a, .filter-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    if (event?.target) {
        event.target.classList.add('active');
    }
}

// Create Product Card
function createProductCard(product) {
    return `
        <div class="product-card fade-in" onclick="showProductDetail(${product.id})">
            <div class="product-image">
                <div style="background: #1a1a1a; height: 100%; display: flex; align-items: center; justify-content: center; color: #666; padding: 20px; text-align: center;">
                    ${product.name.substring(0, 40)}
                </div>
                ${product.badge ? `<div class="product-badge">${product.badge}</div>` : ''}
                <div class="quick-add" onclick="quickAddToCart(${product.id}, event)">
                    SEPETE EKLE
                </div>
            </div>
            <div class="product-info">
                <div class="product-name">${product.name}</div>
                <div class="product-price">
                    ${product.oldPrice ? `<span class="old-price">₺${formatPrice(product.oldPrice)}</span>` : ''}
                    <span class="${product.oldPrice ? 'new-price' : ''}">₺${formatPrice(product.price)}</span>
                </div>
            </div>
        </div>
    `;
}

// Override handleLogin
const originalHandleLogin = window.handleLogin;
window.handleLogin = async function(event) {
    event.preventDefault();
    
    const form = event.target;
    const email = form.querySelector('input[type="email"]').value;
    const password = form.querySelector('input[type="password"]').value;
    
    try {
        // Mock login - In production, connect to IdentityServer
        const user = {
            email: email,
            role: email.toLowerCase().includes('admin') ? 'Admin' : 'Customer',
            token: 'mock-token-' + Date.now()
        };
        
        localStorage.setItem('frnch_user', JSON.stringify(user));
        localStorage.setItem('frnch_token', user.token);
        
        currentUser = user;
        isAuthenticated = true;
        isAdmin = user.role === 'Admin';
        
        showNotification('Giriş başarılı!');
        updateAuthUI();
        showPage('mainShop');
    } catch (error) {
        console.error('Login error:', error);
        showNotification('Giriş başarısız');
    }
};

// Override showPage to protect admin panel
const originalShowPage = window.showPage;
window.showPage = function(pageName) {
    // Check if trying to access admin panel without authorization
    if (pageName === 'admin' && !isAdmin) {
        showNotification('Admin yetkisi gereklidir!');
        showPage('login');
        return;
    }
    
    // Call original function
    if (originalShowPage) {
        originalShowPage(pageName);
    }
    
    // Load admin data if opening admin panel
    if (pageName === 'admin' && isAdmin) {
        loadAdminData();
    }
};

// Load Admin Data
async function loadAdminData() {
    try {
        const ordersResponse = await fetch(`${API_BASE}/api/order/orders`);
        if (ordersResponse.ok) {
            const orders = await ordersResponse.json();
            console.log('📦 Orders loaded:', orders.length);
            
            const tbody = document.querySelector('#ordersSection tbody');
            if (tbody && orders.length > 0) {
                tbody.innerHTML = orders.map(order => `
                    <tr>
                        <td>#${order.orderId || order.orderingId}</td>
                        <td>${new Date(order.orderDate).toLocaleDateString('tr-TR')}</td>
                        <td>${order.userId || 'N/A'}</td>
                        <td>₺${formatPrice(order.totalPrice)}</td>
                        <td>💳 Paid</td>
                        <td><span class="status-badge success">Completed</span></td>
                        <td>Order details</td>
                    </tr>
                `).join('');
            }
        }
    } catch (error) {
        console.error('Error loading admin data:', error);
    }
}

// Override completeOrder
const originalCompleteOrder = window.completeOrder;
window.completeOrder = async function() {
    if (cart.length === 0) {
        showNotification('Sepetiniz boş!');
        return;
    }
    
    if (!isAuthenticated) {
        showNotification('Sipariş vermek için giriş yapmalısınız');
        showPage('login');
        return;
    }
    
    try {
        const orderData = {
            userId: currentUser.email,
            orderDate: new Date().toISOString(),
            totalPrice: cart.reduce((sum, item) => sum + (item.price * item.quantity), 0)
        };
        
        showNotification(`Siparişiniz alındı! Sipariş #${Math.floor(Math.random() * 10000)}`);
        cart = [];
        updateCartUI();
        showPage('mainShop');
    } catch (error) {
        console.error('Order error:', error);
        showNotification('Sipariş oluşturulamadı');
    }
};

// Initialize on DOM load
document.addEventListener('DOMContentLoaded', () => {
    console.log('🎨 FRNCH Shop Loading...');
    setTimeout(() => {
        initializeBackend();
    }, 100);
});

// Export for debugging
window.backendIntegration = {
    initializeBackend,
    loadProductsFromBackend,
    loadCategoriesFromBackend,
    currentUser: () => currentUser,
    isAuthenticated: () => isAuthenticated,
    isAdmin: () => isAdmin,
    logout
};

console.log('✅ Backend integration loaded');