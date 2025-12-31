using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Features.CQRS.Commands.OrderingDetailCommands
{
    public class RemoveOrderingDetailCommand
    {
        public int Id { get; set; }
        public RemoveOrderingDetailCommand(int id)
        {
            Id = id;
        }
    }
}
