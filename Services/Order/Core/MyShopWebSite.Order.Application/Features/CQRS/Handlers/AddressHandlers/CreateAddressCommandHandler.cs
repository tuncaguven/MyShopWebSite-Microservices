using MediatR;
using MyShopWebSite.Order.Application.Features.CQRS.Commands.AddressCommands;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class CreateAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;
        private readonly IMediator _mediator;

        public CreateAddressCommandHandler(IRepository<Address> repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(CreateAddressCommand request)
        {
            Address newAddress = new Address
            {
                UserId = request.UserId,
                District = request.District,
                City = request.City,
                Detail = request.Detail
            };
             _repository.Add(newAddress);
            await _repository.SaveChangesAsync();
        }
    }
}
