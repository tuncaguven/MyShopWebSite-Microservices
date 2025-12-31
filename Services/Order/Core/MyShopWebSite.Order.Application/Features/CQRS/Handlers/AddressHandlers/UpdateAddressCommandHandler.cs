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
    public class UpdateAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;
        public UpdateAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateAddressCommand request)
        {
            var address = await _repository.GetByIdAsync(request.AddressId);
            address.District = request.District;
            address.City = request.City;
            address.Detail = request.Detail;
            _repository.Update(address);
            await _repository.SaveChangesAsync();
        }
    }
}
