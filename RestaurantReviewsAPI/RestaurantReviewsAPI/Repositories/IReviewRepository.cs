using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public interface IReviewRepository: IRepository<Review>
    {
        IList<Review> GetReviewsByRestaurant(int id);
        Review GetReviewOfRestaurantForUser(ApplicationUser user, Restaurant restaurant);
        IList<Review> GetReviewsByUser(string userId);
        IList<Review> GetReviews();
        Task<Review> GetReview(int id);
    }
}
