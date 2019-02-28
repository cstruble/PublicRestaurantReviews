using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public IList<Review> GetReviews()
        {
            return _db.Reviews.ToList();
        }
        public IList<Review> GetReviewsByRestaurant(int id)
        {
            return _db.Reviews.Include(rev => rev.Restaurant)
                .Include(rev => rev.User)
                .Where(rev => rev.Restaurant.Id == id).ToList();

        }

        public Review GetReviewOfRestaurantForUser(ApplicationUser user, Restaurant restaurant)
        {
            if (user == null || restaurant == null)
                return null;
            IList<Review> reviewsByRestaurant = GetReviewsByRestaurant(restaurant.Id);
            return reviewsByRestaurant.Where(rev => rev.User == user).SingleOrDefault();
        }

        public IList<Review> GetReviewsByUser(string userId)
        {
            return _db.Reviews.Include(rev => rev.Restaurant)
                .Include(rev => rev.User)
                .Where(rev => rev.User.Id == userId).ToList();
        }

        public async Task<Review> GetReview(int id)
        {
            return await _db.Reviews.Include(rev => rev.Restaurant).SingleOrDefaultAsync(res => res.Id == id);
        }
   }
}