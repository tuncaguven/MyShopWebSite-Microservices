using MediatR;
using MyShopWebSite.Order.Application.Features.Mediator.Queries;
using MyShopWebSite.Order.Application.Features.Mediator.Results;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.Mediator.Handlers
{
    public class GetOrderingQueryHandler : IRequestHandler<GetOrderingQuery,List<GetOrderingQueryResult>>
    {
        private readonly IRepository<Ordering> _repository;

        public GetOrderingQueryHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOrderingQueryResult>> Handle(GetOrderingQuery request, CancellationToken cancellationToken)
        {
           var values = await _repository.GetAllAsync();
            var result = values.Select(o => new GetOrderingQueryResult
            {
                OrderingId = o.OrderingId,
                UserId = o.UserId,
                TotalPrice = o.TotalPrice,
                OrderDate = o.OrderDate
            }).ToList();
            return result;
        }
    }
}
