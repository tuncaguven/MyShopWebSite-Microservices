using MediatR;
using MyShopWebSite.Order.Application.Features.Mediator.Commands;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.Mediator.Handlers
{
    public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand>
    {
        private readonly IRepository<Ordering> _repository;

        public CreateOrderingCommandHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateOrderingCommand request, CancellationToken cancellationToken)
        {
            _repository.Add(new Ordering
            {
                OrderDate = request.OrderDate,
                TotalPrice = request.TotalPrice,
                UserId = request.UserId
            });
            await _repository.SaveChangesAsync();
        }
    }
}
