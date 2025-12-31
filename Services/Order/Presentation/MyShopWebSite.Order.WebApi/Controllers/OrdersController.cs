using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopWebSite.Order.Application.Features.Mediator.Commands;
using MyShopWebSite.Order.Application.Features.Mediator.Queries;

namespace MyShopWebSite.Order.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            // For demo purposes, return sample orders
            var sampleOrders = new[]
            {
                new
                {
                    OrderId = 1,
                    UserId = "user-123",
                    OrderDate = DateTime.Now.AddDays(-5),
                    TotalPrice = 1359.97m,
                    Status = "Delivered",
                    Items = new[]
                    {
                        new { ProductName = "Laptop", Quantity = 1, Price = 1299.99m },
                        new { ProductName = "Mouse", Quantity = 2, Price = 29.99m }
                    }
                },
                new
                {
                    OrderId = 2,
                    UserId = "user-456",
                    OrderDate = DateTime.Now.AddDays(-2),
                    TotalPrice = 89.99m,
                    Status = "Processing",
                    Items = new[]
                    {
                        new { ProductName = "Keyboard", Quantity = 1, Price = 89.99m }
                    }
                }
            };
            return Ok(sampleOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetOrderingByIdQuery(id));
                return Ok(result);
            }
            catch
            {
                return NotFound(new { message = "Sipariþ bulunamadý" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderingCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Sipariþ baþarýyla oluþturuldu" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _mediator.Send(new RemoveOrderingCommand(id));
            return Ok(new { message = "Sipariþ baþarýyla silindi" });
        }
    }
}