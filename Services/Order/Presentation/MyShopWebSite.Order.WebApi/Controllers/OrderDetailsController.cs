using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopWebSite.Order.Application.Features.CQRS.Commands.OrderingDetailCommands;
using MyShopWebSite.Order.Application.Features.CQRS.Handlers.OrderingDetailHandlers;
using MyShopWebSite.Order.Application.Features.CQRS.Queries.OrderingDetailResult;

namespace MyShopWebSite.Order.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly GetOrderingDetailQueryHandler _getOrderingDetailQueryHandler;
        private readonly GetOrderingDetailByIdQueryHandler _getOrderingDetailByIdQueryHandler;
        private readonly CreateOrderingDetailCommandHandler _createOrderingDetailCommandHandler;
        private readonly UpdateOrderingDetailCommandHandler _updateOrderingDetailCommandHandler;
        private readonly DeleteOrderingDetailCommandHandler _deleteOrderingDetailCommandHandler;

        public OrderDetailsController(GetOrderingDetailQueryHandler getOrderingDetailQueryHandler, GetOrderingDetailByIdQueryHandler getOrderingDetailByIdQueryHandler, CreateOrderingDetailCommandHandler createOrderingDetailCommandHandler, UpdateOrderingDetailCommandHandler updateOrderingDetailCommandHandler, DeleteOrderingDetailCommandHandler deleteOrderingDetailCommandHandler)
        {
            _getOrderingDetailQueryHandler = getOrderingDetailQueryHandler;
            _getOrderingDetailByIdQueryHandler = getOrderingDetailByIdQueryHandler;
            _createOrderingDetailCommandHandler = createOrderingDetailCommandHandler;
            _updateOrderingDetailCommandHandler = updateOrderingDetailCommandHandler;
            _deleteOrderingDetailCommandHandler = deleteOrderingDetailCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetailList()
        {
            var result = await _getOrderingDetailQueryHandler.Handle();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> OrderDetailById(int id)
        {
            var result = await _getOrderingDetailByIdQueryHandler.Handle(new GetOrderingDetailByIdQuery(id));
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail([FromBody] CreateOrderingDetailCommand command)
        {
            await _createOrderingDetailCommandHandler.Handle(command);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetail([FromBody] UpdateOrderingDetailCommand command)
        {
            await _updateOrderingDetailCommandHandler.Handle(command);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            await _deleteOrderingDetailCommandHandler.Handle(new RemoveOrderingDetailCommand(id));
            return Ok();
        }

    }
}
