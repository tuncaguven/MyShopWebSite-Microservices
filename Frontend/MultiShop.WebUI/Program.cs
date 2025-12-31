using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services;
using Microsoft.Extensions.Options;

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

// Register HttpClient for TokenService
builder.Services.AddHttpClient<ITokenService, TokenService>();

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

app.UseAuthorization();

// Serve index.html as default - UPDATE THIS
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

app.MapControllers();

app.Run();
