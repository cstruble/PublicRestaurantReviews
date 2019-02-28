using RestaurantReviewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public interface IRepository<T>
    {
        bool Update(int id, T entity);
        Task<T> Add(T entity);
        T Get(int id);
        void SetContext(ApplicationDbContext context);
        Task<bool> Delete(int id);
    }
}
