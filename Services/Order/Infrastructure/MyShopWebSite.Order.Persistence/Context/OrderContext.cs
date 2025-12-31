using Microsoft.EntityFrameworkCore;
using MyShopWebSite.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Persistence.Context
{
    public class OrderContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
    "Server=localhost,1434;Database=OrderDb;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;");

        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Ordering> Ordering { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
