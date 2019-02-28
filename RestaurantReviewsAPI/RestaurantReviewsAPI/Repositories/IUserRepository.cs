using RestaurantReviewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public interface IUserRepository
    {
        void SetContext(ApplicationDbContext context);
        ApplicationUser GetUserByUserId(string userId);
    }
}
