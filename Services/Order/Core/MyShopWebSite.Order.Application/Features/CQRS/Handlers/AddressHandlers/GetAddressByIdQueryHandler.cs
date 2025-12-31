using MyShopWebSite.Order.Application.Features.CQRS.Queries.AddressQueries;
using MyShopWebSite.Order.Application.Features.CQRS.Results.AddressResults;
using MyShopWebSite.Order.Application.Interfaces;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class GetAddressByIdQueryHandler
    {
        private readonly IRepository<Address> _repository;

        public GetAddressByIdQueryHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }
        public async Task<GetAddressByIdQueryResult> Handle(GetAddressByIdQuery query)
        {
            var address =  await _repository.GetByIdAsync(query.Id);
           return new GetAddressByIdQueryResult 
           { 
               AddressId=address.AddressId,
               UserId=address.UserId,
               District=address.District,
               City=address.City,
               Detail=address.Detail
           };
        }
    }
}
