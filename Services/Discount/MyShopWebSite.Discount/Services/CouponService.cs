using Dapper;
using MyShopWebSite.Discount.Context;
using MyShopWebSite.Discount.Dtos;

namespace MyShopWebSite.Discount.Services
{
    public class CouponService : ICouponService
    {
        private readonly DapperContext _context;

        public CouponService(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CreateCouponDto createCouponDto)
        {
            string query = "insert into Coupons (Code,Rate,IsActive,ValidDate) values (@code,@rate,@isActive,@validDate)";
            var parameters = new DynamicParameters();
            parameters.Add("code", createCouponDto.Code);
            parameters.Add("rate", createCouponDto.Rate);
            parameters.Add("isActive", createCouponDto.IsActive);
            parameters.Add("validDate", createCouponDto.ValidDate);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteAsync(int id)
        {
            string query = "delete From Coupons where CouponID=@couponId";
            var parameters = new DynamicParameters();
            parameters.Add("couponId", id);
            using(var connection = _context.CreateConnection())
            {
               await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultCouponDto>> GetAllCouponAsync()
        {
            string query = "select * from Coupons";
            using (var connection = _context.CreateConnection())
            {
                var coupons = await connection.QueryAsync<ResultCouponDto>(query);
                return coupons.ToList();
            }
        }

        public async Task<ResultCouponDto> GetByIdAsync(int id)
        {
            string query = "select * from Coupons where CouponID=@couponId";
            var parameters = new DynamicParameters();
            parameters.Add("couponId", id);
            using (var connection = _context.CreateConnection())
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<ResultCouponDto>(query, parameters);
                return coupon;
            }
        }

        public async Task UpdateAsync(UpdateCouponDto updateCouponDto)
        {
           string query = "update Coupons set Code=@code, Rate=@rate, IsActive=@isActive, ValidDate=@validDate where CouponID=@couponId";
            var parameters = new DynamicParameters();
            parameters.Add("couponId", updateCouponDto.CouponID);
            parameters.Add("code", updateCouponDto.Code);
            parameters.Add("rate", updateCouponDto.Rate);
            parameters.Add("isActive", updateCouponDto.IsActive);
            parameters.Add("validDate", updateCouponDto.ValidDate);
            using (var connection = _context.CreateConnection())
            {
                 await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
