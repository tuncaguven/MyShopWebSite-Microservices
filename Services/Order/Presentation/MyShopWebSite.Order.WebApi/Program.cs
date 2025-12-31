using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyShopWebSite.Order.Application.Features.CQRS.Handlers.AddressHandlers;
using MyShopWebSite.Order.Application.Features.CQRS.Handlers.OrderingDetailHandlers;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Application.Services;
using MyShopWebSite.Order.Persistence.Context;
using MyShopWebSite.Order.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>

{

    options.Authority = builder.Configuration["IdentityServerUrl"];

    options.Audience = "ResourceOrder";

    options.RequireHttpsMetadata = false;



    options.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuerSigningKey = true,

        ValidateIssuer = true,

        ValidIssuer = builder.Configuration["IdentityServerUrl"],

        ValidateAudience = true,

        ValidAudience = "ResourceOrder",

        ValidateLifetime = true,

        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>

        {

            var client = new HttpClient();

            var response = client.GetStringAsync($"{builder.Configuration["IdentityServerUrl"]}/.well-known/openid-configuration/jwks").Result;

            var keys = new JsonWebKeySet(response);

            return keys.Keys;

        }

    };

});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddDbContext<OrderContext>();

#region
builder.Services.AddScoped<GetAddressQueryHandler>();
builder.Services.AddScoped<GetAddressByIdQueryHandler>();
builder.Services.AddScoped<CreateAddressCommandHandler>();
builder.Services.AddScoped<UpdateAddressCommandHandler>();
builder.Services.AddScoped<RemoveAddressCommandHandler>();

builder.Services.AddScoped<GetOrderingDetailQueryHandler>();
builder.Services.AddScoped<GetOrderingDetailByIdQueryHandler>();
builder.Services.AddScoped<CreateOrderingDetailCommandHandler>();
builder.Services.AddScoped<UpdateOrderingDetailCommandHandler>();
builder.Services.AddScoped<DeleteOrderingDetailCommandHandler>();
#endregion

// Add services to the container.

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

app.Run();
