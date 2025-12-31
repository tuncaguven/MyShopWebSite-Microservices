using MyShopWebSite.Order.Application.Features.CQRS.Commands.OrderingDetailCommands;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Handlers.OrderingDetailHandlers
{
    public class CreateOrderingDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public CreateOrderingDetailCommandHandler(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task Handle(CreateOrderingDetailCommand request)
        {
            OrderDetail orderDetail = new()
            {
                ProductID = request.ProductID,
                ProductName = request.ProductName,
                ProductPrice = request.ProductPrice,
                ProductAmount = request.ProductAmount,
                ProductTotalPrice = request.ProductTotalPrice,
                OrderingId = request.OrderingId
            };
             _orderDetailRepository.Add(orderDetail);
            await _orderDetailRepository.SaveChangesAsync();
        }
    }
}
