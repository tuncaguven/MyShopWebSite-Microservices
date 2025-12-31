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
    public class GetOrderingByIdHandler : IRequestHandler<GetOrderingByIdQuery, GetOrderingByIdQueryResult>
    {
        private readonly IRepository<Ordering> _repository;
        public GetOrderingByIdHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task<GetOrderingByIdQueryResult> Handle(GetOrderingByIdQuery request, CancellationToken cancellationToken)
        {
            var ordering = await _repository.GetByIdAsync(request.Id);

            return new GetOrderingByIdQueryResult
            {
                OrderDate = ordering.OrderDate,
                OrderingId = ordering.OrderingId,
                TotalPrice = ordering.TotalPrice,
                UserId = ordering.UserId
            };
            
        }
    }
}
