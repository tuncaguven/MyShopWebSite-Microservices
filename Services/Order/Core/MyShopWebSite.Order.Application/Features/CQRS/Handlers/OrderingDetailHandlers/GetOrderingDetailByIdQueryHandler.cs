using MyShopWebSite.Order.Application.Features.CQRS.Queries.AddressQueries;
using MyShopWebSite.Order.Application.Features.CQRS.Queries.OrderingDetailResult;
using MyShopWebSite.Order.Application.Features.CQRS.Results.OrderingDetailResult;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Handlers.OrderingDetailHandlers
{
    public class GetOrderingDetailByIdQueryHandler
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public GetOrderingDetailByIdQueryHandler(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<GetOrderingDetailByIdQueryResult> Handle(GetOrderingDetailByIdQuery query)
        {
          var values = await _orderDetailRepository.GetByFilterAsync(od => od.OrderingId == query.Id);
            var result = new GetOrderingDetailByIdQueryResult
            {
                OrderDetailId = values.OrderDetailId,
                ProductID = values.ProductID,
                ProductName = values.ProductName,
                ProductPrice = values.ProductPrice,
                ProductAmount = values.ProductAmount,
                ProductTotalPrice = values.ProductTotalPrice,
                OrderingId = values.OrderingId

            };
            return result;
        }   
    }
}
