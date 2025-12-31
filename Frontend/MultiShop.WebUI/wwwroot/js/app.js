// API Base URL (MVC proxy)
const API_BASE = '';

// Utility function to display results
function displayResult(serviceName, data, isError = false) {
    const resultsDiv = document.getElementById('results');
    const timestamp = new Date().toLocaleTimeString();
    
    let message = `[${timestamp}] ${serviceName}\n`;
    message += '─'.repeat(50) + '\n';
    
    if (isError) {
        message += '❌ ERROR:\n';
        message += typeof data === 'string' ? data : JSON.stringify(data, null, 2);
    } else {
        message += '✅ SUCCESS:\n';
        message += JSON.stringify(data, null, 2);
    }
    
    message += '\n' + '═'.repeat(50) + '\n\n';
    
    resultsDiv.textContent = message + resultsDiv.textContent;
}

// Generic API call function
async function callApi(endpoint, serviceName) {
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(btn => btn.disabled = true);
    
    const resultsDiv = document.getElementById('results');
    resultsDiv.textContent = `⏳ Calling ${serviceName}...\n\n` + resultsDiv.textContent;
    
    try {
        const response = await fetch(endpoint, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        const contentType = response.headers.get('content-type');
        let data;
        
        if (contentType && contentType.includes('application/json')) {
            data = await response.json();
        } else {
            data = await response.text();
            // Try to parse as JSON if it looks like JSON
            try {
                data = JSON.parse(data);
            } catch (e) {
                // Keep as text if not valid JSON
            }
        }
        
        if (!response.ok) {
            displayResult(serviceName, {
                status: response.status,
                statusText: response.statusText,
                error: data
            }, true);
        } else {
            displayResult(serviceName, data, false);
        }
    } catch (error) {
        displayResult(serviceName, {
            message: error.message,
            type: error.name
        }, true);
    } finally {
        buttons.forEach(btn => btn.disabled = false);
    }
}

// Shopping Cart State
let cart = [];
let products = [];
let categories = [];

// Initialize
document.addEventListener('DOMContentLoaded', async () => {
    console.log('🛒 MultiShop Loading...');
    await loadCategories();
    await loadProducts();
    updateCartUI();
});

// Load Categories from Backend
async function loadCategories() {
    try {
        const response = await fetch('/api/catalog/categories');
        if (response.ok) {
            categories = await response.json();
            displayCategories();
        }
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

// Display Categories
function displayCategories() {
    const container = document.getElementById('categoriesContainer');
    
    const categoryButtons = `
        <button class="category-btn active" onclick="filterByCategory('all')">All Products</button>
        ${categories.map(cat => `
            <button class="category-btn" onclick="filterByCategory('${cat.categoryId}')">${cat.categoryName}</button>
        `).join('')}
    `;
    
    container.innerHTML = categoryButtons;
}

// Load Products from Backend
async function loadProducts() {
    try {
        const response = await fetch('/api/catalog/products');
        if (response.ok) {
            products = await response.json();
            displayProducts(products);
        }
    } catch (error) {
        console.error('Error loading products:', error);
        document.getElementById('productsContainer').innerHTML = `
            <div style="text-align: center; padding: 2rem; grid-column: 1/-1;">
                <p style="color: var(--danger);">Failed to load products. Please try again later.</p>
            </div>
        `;
    }
}

// Display Products
function displayProducts(productsToDisplay) {
    const container = document.getElementById('productsContainer');
    
    if (productsToDisplay.length === 0) {
        container.innerHTML = '<div style="text-align: center; padding: 2rem; grid-column: 1/-1;">No products found</div>';
        return;
    }
    
    container.innerHTML = productsToDisplay.map(product => `
        <div class="product-card">
            <div class="product-image">
                ${product.productName ? product.productName.substring(0, 50) : 'Product'}
            </div>
            <div class="product-info">
                <div class="product-name">${product.productName || 'Unnamed Product'}</div>
                <div class="product-description">${product.productDescription || 'No description available'}</div>
                <div class="product-price">$${(product.productPrice || 0).toFixed(2)}</div>
                <button class="add-to-cart-btn" onclick='addToCart(${JSON.stringify(product).replace(/'/g, "&apos;")})'>
                    Add to Cart
                </button>
            </div>
        </div>
    `).join('');
}

// Filter by Category
function filterByCategory(categoryId) {
    // Update active button
    document.querySelectorAll('.category-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    
    // Filter products
    const filtered = categoryId === 'all' 
        ? products 
        : products.filter(p => p.categoryId === categoryId);
    
    displayProducts(filtered);
}

// Add to Cart
function addToCart(product) {
    const existingItem = cart.find(item => item.productId === product.productId);
    
    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({
            productId: product.productId,
            productName: product.productName,
            productPrice: product.productPrice,
            quantity: 1
        });
    }
    
    updateCartUI();
    showNotification('Product added to cart!');
}

// Update Cart UI
function updateCartUI() {
    const cartCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartTotal = cart.reduce((sum, item) => sum + (item.productPrice * item.quantity), 0);
    
    document.getElementById('cartCount').textContent = cartCount;
    document.getElementById('cartTotal').textContent = `$${cartTotal.toFixed(2)}`;
    
    const cartItemsContainer = document.getElementById('cartItems');
    
    if (cart.length === 0) {
        cartItemsContainer.innerHTML = '<p style="text-align: center; color: var(--gray);">Your cart is empty</p>';
    } else {
        cartItemsContainer.innerHTML = cart.map(item => `
            <div class="cart-item">
                <div class="cart-item-image"></div>
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.productName}</div>
                    <div class="cart-item-price">$${item.productPrice.toFixed(2)}</div>
                    <div class="cart-item-quantity">
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', -1)">-</button>
                        <span>${item.quantity}</span>
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', 1)">+</button>
                        <button class="qty-btn" onclick="removeFromCart('${item.productId}')" style="margin-left: auto;">🗑️</button>
                    </div>
                </div>
            </div>
        `).join('');
    }
}

// Update Quantity
function updateQuantity(productId, delta) {
    const item = cart.find(i => i.productId === productId);
    if (item) {
        item.quantity = Math.max(1, item.quantity + delta);
        updateCartUI();
    }
}

// Remove from Cart
function removeFromCart(productId) {
    cart = cart.filter(item => item.productId !== productId);
    updateCartUI();
}

// Toggle Cart
function toggleCart() {
    document.getElementById('cartSidebar').classList.toggle('open');
    document.getElementById('overlay').classList.toggle('active');
}

// Checkout
async function checkout() {
    if (cart.length === 0) {
        showNotification('Your cart is empty!', 'error');
        return;
    }
    
    try {
        const orderData = {
            userId: 'demo-user',
            orderDate: new Date().toISOString(),
            totalPrice: cart.reduce((sum, item) => sum + (item.productPrice * item.quantity), 0),
            items: cart
        };
        
        const response = await fetch('/api/order/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(orderData)
        });
        
        if (response.ok) {
            showNotification('Order placed successfully!');
            cart = [];
            updateCartUI();
            toggleCart();
        } else {
            showNotification('Order failed. Please try again.', 'error');
        }
    } catch (error) {
        console.error('Checkout error:', error);
        showNotification('Order failed. Please try again.', 'error');
    }
}

// Show Notification
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = 'notification';
    notification.textContent = message;
    notification.style.background = type === 'error' ? 'var(--danger)' : 'var(--success)';
    
    document.body.appendChild(notification);
    
    setTimeout(() => notification.remove(), 3000);
}

// Shopping Cart State
let cart = [];
let products = [];
let categories = [];

// Initialize
document.addEventListener('DOMContentLoaded', async () => {
    console.log('🛒 MultiShop Loading...');
    await loadCategories();
    await loadProducts();
    updateCartUI();
});

// Load Categories from Backend
async function loadCategories() {
    try {
        const response = await fetch('/api/catalog/categories');
        if (response.ok) {
            categories = await response.json();
            displayCategories();
        }
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

// Display Categories
function displayCategories() {
    const container = document.getElementById('categoriesContainer');
    
    const categoryButtons = `
        <button class="category-btn active" onclick="filterByCategory('all')">All Products</button>
        ${categories.map(cat => `
            <button class="category-btn" onclick="filterByCategory('${cat.categoryId}')">${cat.categoryName}</button>
        `).join('')}
    `;
    
    container.innerHTML = categoryButtons;
}

// Load Products from Backend
async function loadProducts() {
    try {
        const response = await fetch('/api/catalog/products');
        if (response.ok) {
            products = await response.json();
            displayProducts(products);
        }
    } catch (error) {
        console.error('Error loading products:', error);
        document.getElementById('productsContainer').innerHTML = `
            <div style="text-align: center; padding: 2rem; grid-column: 1/-1;">
                <p style="color: var(--danger);">Failed to load products. Please try again later.</p>
            </div>
        `;
    }
}

// Display Products
function displayProducts(productsToDisplay) {
    const container = document.getElementById('productsContainer');
    
    if (productsToDisplay.length === 0) {
        container.innerHTML = '<div style="text-align: center; padding: 2rem; grid-column: 1/-1;">No products found</div>';
        return;
    }
    
    container.innerHTML = productsToDisplay.map(product => `
        <div class="product-card">
            <div class="product-image">
                ${product.productName ? product.productName.substring(0, 50) : 'Product'}
            </div>
            <div class="product-info">
                <div class="product-name">${product.productName || 'Unnamed Product'}</div>
                <div class="product-description">${product.productDescription || 'No description available'}</div>
                <div class="product-price">$${(product.productPrice || 0).toFixed(2)}</div>
                <button class="add-to-cart-btn" onclick='addToCart(${JSON.stringify(product).replace(/'/g, "&apos;")})'>
                    Add to Cart
                </button>
            </div>
        </div>
    `).join('');
}

// Filter by Category
function filterByCategory(categoryId) {
    // Update active button
    document.querySelectorAll('.category-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    
    // Filter products
    const filtered = categoryId === 'all' 
        ? products 
        : products.filter(p => p.categoryId === categoryId);
    
    displayProducts(filtered);
}

// Add to Cart
function addToCart(product) {
    const existingItem = cart.find(item => item.productId === product.productId);
    
    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({
            productId: product.productId,
            productName: product.productName,
            productPrice: product.productPrice,
            quantity: 1
        });
    }
    
    updateCartUI();
    showNotification('Product added to cart!');
}

// Update Cart UI
function updateCartUI() {
    const cartCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartTotal = cart.reduce((sum, item) => sum + (item.productPrice * item.quantity), 0);
    
    document.getElementById('cartCount').textContent = cartCount;
    document.getElementById('cartTotal').textContent = `$${cartTotal.toFixed(2)}`;
    
    const cartItemsContainer = document.getElementById('cartItems');
    
    if (cart.length === 0) {
        cartItemsContainer.innerHTML = '<p style="text-align: center; color: var(--gray);">Your cart is empty</p>';
    } else {
        cartItemsContainer.innerHTML = cart.map(item => `
            <div class="cart-item">
                <div class="cart-item-image"></div>
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.productName}</div>
                    <div class="cart-item-price">$${item.productPrice.toFixed(2)}</div>
                    <div class="cart-item-quantity">
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', -1)">-</button>
                        <span>${item.quantity}</span>
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', 1)">+</button>
                        <button class="qty-btn" onclick="removeFromCart('${item.productId}')" style="margin-left: auto;">🗑️</button>
                    </div>
                </div>
            </div>
        `).join('');
    }
}

// Update Quantity
function updateQuantity(productId, delta) {
    const item = cart.find(i => i.productId === productId);
    if (item) {
        item.quantity = Math.max(1, item.quantity + delta);
        updateCartUI();
    }
}

// Remove from Cart
function removeFromCart(productId) {
    cart = cart.filter(item => item.productId !== productId);
    updateCartUI();
}

// Toggle Cart
function toggleCart() {
    document.getElementById('cartSidebar').classList.toggle('open');
    document.getElementById('overlay').classList.toggle('active');
}

// Checkout
async function checkout() {
    if (cart.length === 0) {
        showNotification('Your cart is empty!', 'error');
        return;
    }
    
    try {
        const orderData = {
            userId: 'demo-user',
            orderDate: new Date().toISOString(),
            totalPrice: cart.reduce((sum, item) => sum + (item.productPrice * item.quantity), 0),
            items: cart
        };
        
        const response = await fetch('/api/order/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(orderData)
        });
        
        if (response.ok) {
            showNotification('Order placed successfully!');
            cart = [];
            updateCartUI();
            toggleCart();
        } else {
            showNotification('Order failed. Please try again.', 'error');
        }
    } catch (error) {
        console.error('Checkout error:', error);
        showNotification('Order failed. Please try again.', 'error');
    }
}

// Show Notification
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = 'notification';
    notification.textContent = message;
    notification.style.background = type === 'error' ? 'var(--danger)' : 'var(--success)';
    
    document.body.appendChild(notification);
    
    setTimeout(() => notification.remove(), 3000);
}

// Shopping Cart State
let cart = [];
let products = [];
let categories = [];

// Initialize
document.addEventListener('DOMContentLoaded', async () => {
    console.log('🛒 MultiShop Loading...');
    await loadCategories();
    await loadProducts();
    updateCartUI();
});

// Load Categories from Backend
async function loadCategories() {
    try {
        const response = await fetch('/api/catalog/categories');
        if (response.ok) {
            categories = await response.json();
            displayCategories();
        }
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

// Display Categories
function displayCategories() {
    const container = document.getElementById('categoriesContainer');
    
    const categoryButtons = `
        <button class="category-btn active" onclick="filterByCategory('all')">All Products</button>
        ${categories.map(cat => `
            <button class="category-btn" onclick="filterByCategory('${cat.categoryId}')">${cat.categoryName}</button>
        `).join('')}
    `;
    
    container.innerHTML = categoryButtons;
}

// Display Products
function displayProducts(productsToDisplay) {
    const container = document.getElementById('productsContainer');
    
    if (productsToDisplay.length === 0) {
        container.innerHTML = '<div style="text-align: center; padding: 2rem; grid-column: 1/-1;">No products found</div>';
        return;
    }
    
    container.innerHTML = productsToDisplay.map(product => `
        <div class="product-card">
            <div class="product-image">
                ${product.productName ? product.productName.substring(0, 50) : 'Product'}
            </div>
            <div class="product-info">
                <div class="product-name">${product.productName || 'Unnamed Product'}</div>
                <div class="product-description">${product.productDescription || 'No description available'}</div>
                <div class="product-price">$${(product.productPrice || 0).toFixed(2)}</div>
                <button class="add-to-cart-btn" onclick='addToCart(${JSON.stringify(product).replace(/'/g, "&apos;")})'>
                    Add to Cart
                </button>
            </div>
        </div>
    `).join('// Filter by Category
function filterByCategory(categoryId) {
    // Update active button
    document.querySelectorAll('.category-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    
    // Filter products
    const filtered = categoryId === 'all' 
        ? products 
        : products.filter(p => p.categoryId === categoryId);
    
    displayProducts(filtered);
}

// Update Cart UI
function updateCartUI() {
    const cartCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartTotal = cart.reduce((sum, item) => sum + (item.productPrice * item.quantity), 0);
    
    document.getElementById('cartCount').textContent = cartCount;
    document.getElementById('cartTotal').textContent = `$${cartTotal.toFixed(2)}`;
    
    const cartItemsContainer = document.getElementById('cartItems');
    
    if (cart.length === 0) {
        cartItemsContainer.innerHTML = '<p style="text-align: center; color: var(--gray);">Your cart is empty</p>';
    } else {
        cartItemsContainer.innerHTML = cart.map(item => `
            <div class="cart-item">
                <div class="cart-item-image"></div>
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.productName}</div>
                    <div class="cart-item-price">$${item.productPrice.toFixed(2)}</div>
                    <div class="cart-item-quantity">
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', -1)">-</button>
                        <span>${item.quantity}</span>
                        <button class="qty-btn" onclick="updateQuantity('${item.productId}', 1)">+</button>
                        <button class="qty-btn" onclick="removeFromCart('${item.productId}')" style="margin-left: auto;">🗑️</button>
                    </div>
                </div>
            </div>
        `).join('');
    }
}

function testOrderSecure() {
    callApi(`${API_BASE}/api/order/health/secure`, 'Order - Secure Health Check');
}

// Discount Service Tests
function testDiscount() {
    callApi(`${API_BASE}/api/discount/discounts`, 'Discount - Get All Discounts');
}

function testDiscountSecure() {
    callApi(`${API_BASE}/api/discount/health/secure`, 'Discount - Secure Health Check');
}

// Clear results
function clearResults() {
    document.getElementById('results').textContent = 'Click any button above to test the services...';
}

// Display initial message
console.log('🚀 MultiShop UI loaded successfully!');
console.log('💡 All API calls go through the MVC BFF proxy with server-side authentication');