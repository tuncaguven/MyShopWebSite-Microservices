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
    public class UpdateOrderingDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public UpdateOrderingDetailCommandHandler(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task Handle(UpdateOrderingDetailCommand command)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(command.OrderDetailId);
            if (orderDetail != null)
            {
                orderDetail.ProductID = command.ProductID;
                orderDetail.ProductName = command.ProductName;
                orderDetail.ProductPrice = command.ProductPrice;
                orderDetail.ProductAmount = command.ProductAmount;
                orderDetail.ProductTotalPrice = command.ProductTotalPrice;
                orderDetail.OrderingId = command.OrderingId;
                _orderDetailRepository.Update(orderDetail);
               await _orderDetailRepository.SaveChangesAsync();
            }

        }

    }
}
