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
    public class DeleteOrderingDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public DeleteOrderingDetailCommandHandler(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task Handle(RemoveOrderingDetailCommand query)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(query.Id);
            if (orderDetail != null)
            {
                _orderDetailRepository.Delete(orderDetail);
                await _orderDetailRepository.SaveChangesAsync();
            }
        }
    }
}
