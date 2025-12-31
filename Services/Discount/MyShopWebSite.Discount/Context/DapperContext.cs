using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyShopWebSite.Discount.Entities;
using System.Data;

namespace MyShopWebSite.Discount.Context
{
    public class DapperContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
              "Server=localhost;Initial Catalog=MyShopWebSiteDiscountDb;Integrated Security=true;TrustServerCertificate=True;");
        }

        public DbSet<Coupon> Coupons { get; set; }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
