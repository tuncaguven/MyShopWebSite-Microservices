using Microsoft.AspNetCore.Authentication.Cookies;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure strongly-typed settings
builder.Services.Configure<IdentityServerConfiguration>(
    builder.Configuration.GetSection("IdentityServer"));
builder.Services.Configure<ServiceUrlConfiguration>(
    builder.Configuration.GetSection("ServiceUrls"));

// Add memory cache for token caching
builder.Services.AddMemoryCache();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register HttpClient for TokenService
builder.Services.AddHttpClient<ITokenService, TokenService>();

// Register CatalogApiClient as typed HttpClient
builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>();

// Register HttpClientFactory for API calls
builder.Services.AddHttpClient();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001", "http://localhost:5251")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

// Serve index.html as default (static frontend)
app.MapGet("/", async context =>
{
    var indexPath = Path.Combine(app.Environment.WebRootPath, "index.html");
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("index.html not found");
    }
});

// Map API controllers
app.MapControllers();

app.Run();
