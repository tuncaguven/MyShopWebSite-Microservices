using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShopWebSite.Order.Application.Interfaces
{
    public interface IRepository<T > where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();

        Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter);
    }
}
