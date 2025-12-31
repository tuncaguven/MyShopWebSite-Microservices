using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Handlers.OrderingDetailHandlers
{
    public class GetOrderingDetailQueryHandler
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public GetOrderingDetailQueryHandler(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<List<OrderDetail>> Handle()
        {
            var orderDetails = await _orderDetailRepository.GetAllAsync();
            return orderDetails.ToList();
        }

    }
}
