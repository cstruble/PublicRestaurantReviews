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
    public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context) : base(context) { }

        public IList<Restaurant> GetRestaurants()
        {
            return _db.Restaurants.Include(r => r.Location).ToList();
        }

        public Restaurant GetRestaurant(int id)
        {
            return _db.Restaurants.Include(res => res.Location)
                .SingleOrDefault(res => res.Id == id);
        }

        public IList<Restaurant> GetRestaurantsByLocation(int id)
        {
            return _db.Restaurants.Include(res => res.Location)
                              .Where(res => res.Location.Id == id).ToList();
        }

        public Restaurant GetRestaurantByNameAndLocationId(string name, int locationId)
        {
            return _db.Restaurants
                .SingleOrDefault(res => res.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && res.LocationId == locationId);
        }

        public async Task<Restaurant> AddRestaurant(Restaurant restaurant)
        {
            _db.Locations.Attach(restaurant.Location);
            return await Add(restaurant);
        }

        public bool UpdateRestaurant(int id, Restaurant restaurant)
        {
            _db.Restaurants.Attach(restaurant);
            return Update(id, restaurant);
        }
    }
}