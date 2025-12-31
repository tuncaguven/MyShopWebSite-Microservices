using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Queries.OrderingDetailResult
{
    public class GetOrderingDetailByIdQuery
    {
        public int Id { get; set; }

        public GetOrderingDetailByIdQuery(int id)
        {
            Id = id;
        }
    }
}
