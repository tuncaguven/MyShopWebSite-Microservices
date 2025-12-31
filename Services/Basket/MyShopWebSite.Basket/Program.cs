using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyShopWebSite.Basket.LoginServices;
using MyShopWebSite.Basket.Services;
using MyShopWebSite.Basket.Settings;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:5001";
        options.Audience = "ResourceBasket";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5001",
            ValidAudience = "ResourceBasket",
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
        options.RefreshOnIssuerKeyNotFound = true;
    });

builder.Services.AddAuthorization();

// Configure Redis Settings
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

// Register IRedisSettings
builder.Services.AddSingleton<IRedisSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<RedisSettings>>().Value;
});

// Register RedisService
builder.Services.AddSingleton<RedisService>();

// Register Basket Service
builder.Services.AddScoped<IBasketService, BasketService>();

// Register Login Service
builder.Services.AddHttpClient<ILoginService, LoginService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize Redis connection
var redisService = app.Services.GetRequiredService<RedisService>();
redisService.Connect();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/api/health/secure", () => new { service = "Basket", status = "OK", timestamp = DateTime.UtcNow })
    .RequireAuthorization();

app.Run();
