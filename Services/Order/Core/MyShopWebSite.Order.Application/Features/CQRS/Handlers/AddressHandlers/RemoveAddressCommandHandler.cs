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
    public class RemoveAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;
        public RemoveAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }
        public async Task Handle(RemoveAddressCommand request)
        {
            var address = await _repository.GetByIdAsync(request.Id);
             _repository.Delete(address);
            await _repository.SaveChangesAsync();
        }
    }
}
