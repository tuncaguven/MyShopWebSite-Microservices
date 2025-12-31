using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyShopWebSite.Discount.Context;
using MyShopWebSite.Discount.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"];
        options.Audience = "ResourceDiscount";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddTransient<DapperContext>();
builder.Services.AddScoped<ICouponService, CouponService>();
// Add services to the container


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/api/health/secure", () => new { service = "Discount", status = "OK", timestamp = DateTime.UtcNow })
    .RequireAuthorization();

app.Run();
